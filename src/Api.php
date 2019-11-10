<?php
namespace iggyvolz\lolisetmanager;

use SleekDB\SleekDB;
use iggyvolz\lolisetmanager\Sources\ChampionGGSource;

class Api
{
    const DATA_DIR=__DIR__."/../db";
    private static function getSetsDB():SleekDB
    {
        return SleekDB::store("sets", self::DATA_DIR,[
            "auto_cache" => false
        ]);
    }
    private static function getLock():Lock
    {
        return new Lock(self::DATA_DIR."/lock");
    }
    public function getItemSet(string $name, ?int $which=null):?array
    {
        self::getLock()->lockRead();
        $sets = self::getSetsDB();
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
    public function initialize():void
    {
        self::getLock()->lockWrite();
        $sets = self::getSetsDB();
        $sets->delete();
        $sets->deleteAllCache();
        foreach((new ChampionGGSource())->getSets() as $name => $set) {
            $set->save($sets, $name);
        }
    }
}