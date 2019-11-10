<?php
namespace iggyvolz\lolisetmanager;

use SleekDB\SleekDB;

/**
 * https://web.archive.org/web/20170201032507/https://developer.riotgames.com/docs/item-sets
 */
class ItemSet
{
    /**
     * The name of the item set as you would see it in the drop down.
     */
    public string $title;
    /**
     * Can be custom or global. This field is only used for grouping and sorting item sets. Custom item sets are ordered above global item sets. This field does not govern whether an item set available for every champion. To make an item set available for every champion, the JSON file must be placed an item set in the global folder.
     */
    public string $type="custom";
    /**
     * The map this item set will appear on. Can be any, Summoner's Rift SR, Howling Abyss HA, Twisted Treeline TT, or Crystal Scar CS.
     */
    public string $map="SR";
    /**
     * The mode this item set will appear on. Can be any, CLASSIC, ARAM, or Dominion ODIN.
     */
    public string $mode="any";
    /**
     * Selectively sort this item set above other item sets. Overrides sortrank, but not type. Defaults to false.
     */
    public bool $priority=false;
    /**
     * The order in which this item set will be sorted within a specific type. Item sets are sorted in descending order.
     */
    public int $sortrank=0;
    /**
     * @var ItemSetBlock[]
     * The sections within an item set.
     */
    public array $blocks=[];

    public function addBlock(string $name, array $items):void
    {
        $block = new ItemSetBlock();
        $block->type=$name;
        foreach($items as $id => $count)
        {
            $item = new ItemSetItem();
            $item->id = $id;
            $item->count = $count;
            $block->items[] = $item;
        }
        $this->blocks[] = $block;
    }

    public function save(SleekDB $db, string $name):void
    {
        $set = json_decode(json_encode($this), true);
        $db->insert([
            "name" => $name,
            "set" => $set
        ]);
    }
}