<?php
namespace iggyvolz\lolisetmanager;

/**
 * https://web.archive.org/web/20170201032507/https://developer.riotgames.com/docs/item-sets
 */
class ItemSetBlock
{
    /**
     * The name of the block as you would see it in the item set.
     */
    public string $type;
    /**
     * Use the tutorial formatting when displaying items in the block. All items within the block are separated by a plus sign with the last item being separated by an arrow indicating that the other items build into the last item. Defaults to false.
     */
    public bool $recMath=false;
    /**
     * The minimum required account level for the block to be visible to the player. Defaults to -1 which is equivalent to "any account level."
     */
    public int $minSummonerLevel=-1;
    /**
     * The maximum allowed account level for the block to be visible to the player. Defaults to -1 which is equivalent to "any account level."
     */
    public int $maxSummonerLevel=-1;
    /**
     * Only show the block if the player has equipped a specific summoner spell. Can be any valid summoner spell key, e.g. SummonerDot. Defaults to an empty string. Will not override hideIfSummonerSpell.  Summoner spell data can be viewed through the static data API (https://web.archive.org/web/20170201032507/https://developer.riotgames.com/api/methods#!/968/3327).
     */
    public string $showIfSummonerSpell="";
    /**
     * Hide the block if the player has equipped a specific summoner spell. Can be any valid summoner spell key, e.g. SummonerDot. Defaults to an empty string. Overrides showIfSummonerSpell.  Summoner spell data can be viewed through the static data API (https://web.archive.org/web/20170201032507/https://developer.riotgames.com/api/methods#!/968/3327).
     */
    public string $hideIfSummonerSpell="";
    /**
     * An array of items to be displayed within the block.
     */
    public array $items=[];
}