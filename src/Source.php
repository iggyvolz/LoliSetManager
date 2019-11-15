<?php
namespace iggyvolz\lolisetmanager;

use DOMXPath;
use DOMDocument;
use Iterator;

abstract class Source
{
    public abstract function getSets():Iterator;
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
    protected static function getChampionID(string $name):string
    {
        return self::getChampionIDMap()[$name];
    }
    protected static function getURL(string $url):DOMXPath
    {
        $dom = new DOMDocument();
        $ch = curl_init();
        curl_setopt($ch, CURLOPT_USERAGENT, "Loli Set Manager DEV (https://github.com/iggyvolz/lolisetmanager)");
        curl_setopt($ch, CURLOPT_URL, $url);
        curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
        $response = curl_exec($ch);
        curl_close($ch);
        $prev=libxml_use_internal_errors(true);
        $dom->loadHTML($response);
        libxml_use_internal_errors($prev);
        return new DOMXPath($dom);
    }
    protected static function getJSON(string $url, array $extraHeaders = []):?array
    {
        $dom = new DOMDocument();
        $ch = curl_init();
        curl_setopt($ch, CURLOPT_VERBOSE, true);
        curl_setopt($ch, CURLOPT_USERAGENT, "Loli Set Manager DEV (https://github.com/iggyvolz/lolisetmanager)");
        curl_setopt($ch, CURLOPT_URL, $url);
        curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
        curl_setopt($ch, CURLOPT_HTTPHEADER, $extraHeaders);
        if($extraCurl) {
            $extraCurl($ch);
        }
        $response = curl_exec($ch);
        var_dump($response);
        curl_close($ch);
        return json_decode($response, true);
    }
}