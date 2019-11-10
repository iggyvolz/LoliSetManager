<?php
namespace iggyvolz\lolisetmanager\Sources;

use DOMXPath;
use DOMDocument;
use EmptyIterator;
use iggyvolz\lolisetmanager\ISource;
use iggyvolz\lolisetmanager\ItemSet;
use iggyvolz\lolisetmanager\Collection;

class ChampionGGSource implements ISource
{
    private static ?array $championIDMap=null;
    private static function getChampionIDMap():array
    {
        if(is_null(self::$championIDMap)) {
            $ch = curl_init();
            curl_setopt($ch, CURLOPT_URL, "https://ddragon.leagueoflegends.com/cdn/9.22.1/data/en_US/champion.json");
            curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
            $response = json_decode(curl_exec($ch), true);
            curl_close($ch);
            self::$championIDMap = iterator_to_array((function() use($response){
                foreach($response["data"] as $id => $champ) {
                    yield $champ["name"] => $id;
                }
            })());
        }
        return self::$championIDMap;
    }
    private function getChampionID(string $name):string
    {
        return self::getChampionIDMap()[$name];
    }
    private static function getURL(string $url):DOMXPath
    {
        $dom = new DOMDocument();
        $ch = curl_init();
        curl_setopt($ch, CURLOPT_URL, $url);
        curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
        $response = curl_exec($ch);
        curl_close($ch);
        $prev=libxml_use_internal_errors(true);
        $dom->loadHTML($response);
        libxml_use_internal_errors($prev);
        return new DOMXPath($dom);
    }
    public function getSets():\iterator
    {
        $collection = new Collection();
        $home = self::getURL("https://champion.gg");
        $version = $home->query("//div[@class=\"analysis-holder\"]//strong[1]")[0]->textContent;
        $sets = $home->query("//*[@id=\"home\"]//a[preceding-sibling::*]");
        foreach($sets as $set) {
            $position = trim($set->textContent);
            $champion = trim($home->query("a[1]", $set->parentNode)[0]->textContent);
            echo "Getting $champion $position\n";
            $url = $set->getAttribute("href");
            $page = self::getURL("https://champion.gg$url");
            $itemSet = new ItemSet();
            $itemSet->title = "Champion.GG $version $champion $position";
            foreach($page->query('//div[@class="build-wrapper"]') as $block) {
                $name = $block->previousSibling->previousSibling->textContent;
                $name .= " (".trim($page->query('div[@class="build-text"]', $block)[0]->textContent).")";
                $images = $page->query("*//img", $block);
                $items=[];
                foreach($images as $image) {
                    $src = $image->getAttribute("src");
                    preg_match("@/([0-9]+)\\.png$@", $src, $matches);
                    $items[$matches[1]]=1;
                }
                $itemSet->addBlock($name, $items);
            }
            $champid=self::getChampionID($champion);
            $champid_lower=strtolower($champid);
            $position_lower=strtolower($position);
            yield "@championgg/$champid_lower-$position_lower" => $itemSet;
            $collection->addSet("@championgg/$champid_lower-$position_lower", $champid);
        }
        yield "@championgg/latest" => $collection;
    }
}