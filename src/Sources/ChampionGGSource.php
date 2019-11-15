<?php
namespace iggyvolz\lolisetmanager\Sources;

use EmptyIterator;
use iggyvolz\lolisetmanager\Source;
use iggyvolz\lolisetmanager\ItemSet;
use iggyvolz\lolisetmanager\Collection;

class ChampionGGSource extends Source
{
    public function getSets():\iterator
    {
        $collection = new Collection();
        $home = self::getURL("https://champion.gg");
        $version = $home->query("//div[@class=\"analysis-holder\"]//strong[1]")[0]->textContent;
        $sets = $home->query("//*[@id=\"home\"]//a[preceding-sibling::*]");
        foreach($sets as $set) {
            $position = trim($set->textContent);
            $champion = trim($home->query("a[1]", $set->parentNode)[0]->textContent);
            echo "Getting $champion $position".PHP_EOL;
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