<?php
namespace iggyvolz\lolisetmanager;

use SleekDB\SleekDB;
use iggyvolz\lolisetmanager\Sources\ChampionGGSource;
use iggyvolz\lolisetmanager\Sources\KoreanBuildsSource;

class Api
{
    const DATA_DIR=__DIR__."/../db";
    private static function getSetsDB():SleekDB
    {
        return SleekDB::store("sets", self::DATA_DIR,[
            "auto_cache" => false
        ]);
    }
    private static function getCollectionsDB():SleekDB
    {
        return SleekDB::store("collections", self::DATA_DIR,[
            "auto_cache" => false
        ]);
    }
    private static function getLock():Lock
    {
        return new Lock(self::DATA_DIR."/lock");
    }
    public static function getCollection(string $name):?array
    {
        self::getLock()->lockRead();
        $collections = self::getCollectionsDB();
        $collection = $collections->where("name", "=", $name)->orderBy("desc", "_id")->limit(1)->fetch();
        if(empty($collection)) {
            \http_response_code(404);
            return null;
        }
        return $collection[0]["collection"];
    }
    public static function getItemSet(string $name):?array
    {
        self::getLock()->lockRead();
        $sets = self::getSetsDB();
        $expl = explode("/", $name);
        if(count($expl) === 3 && preg_match("/^[0-9]+$", $expl[2])) {
            $which = intval(array_pop($expl));
            $name = implode("/", $name);
        } elseif(count($expl) === 2) {
            $which = null;
        } else {
            \http_response_code(404);
            return null;
        }
        $condition = $sets->where("name","=",$name);
        if(is_null($which)) {
            $condition = $condition->orderBy("desc", "_id")->limit(1);
        } else {
            $condition = $condition->orderBy("asc", "_id")->skip($which)->limit(1);
        }
        $set = $condition->fetch();
        if(empty($set)) {
            \http_response_code(404);
            return null;
        }
        return $set[0]["set"];
    }
    protected static function initialize():void
    {
        self::getLock()->lockWrite();
        $sets = self::getSetsDB();
        $sets->delete();
        $collections = self::getCollectionsDB();
        $collections->delete();
        $sources = [
            // new ChampionGGSource(),
            new KoreanBuildsSource(),
        ];
        foreach($sources as $source) {
            foreach($source->getSets() as $name => $set) {
                if($set instanceof Collection) {
                    $set->save($collections, $name);
                } else {
                    $set->save($sets, $name);
                }
            }
        }
    }
}