<?php
namespace iggyvolz\lolisetmanager;

use SleekDB\SleekDB;
use JsonSerializable;

/**
 * https://web.archive.org/web/20170201032507/https://developer.riotgames.com/docs/item-sets
 */
class Collection implements JsonSerializable
{
    private $values=[];
    // Use Global for global sets
    public function addSet(string $set, string $champion)
    {
        if(!array_key_exists($champion, $this->values)) {
            $this->values[$champion]=[];
        }
        $this->values[$champion][]=$set;
    }
    public function jsonSerialize():array
    {
        return $this->values;
    }
    public function save(SleekDB $db, string $name):void
    {
        $collection = json_decode(json_encode($this), true);
        $db->insert(["name"=> $name, "collection" => $collection]);
    }
}