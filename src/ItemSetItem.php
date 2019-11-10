<?php
namespace iggyvolz\lolisetmanager;

/**
 * https://web.archive.org/web/20170201032507/https://developer.riotgames.com/docs/item-sets
 */
class ItemSetItem
{
    /**
     * The item id as a string, e.g. "1001".
     */
    public string $id;
    /**
     * The number of times this item should be purchased. The count is displayed in the bottom right of the item icon. The indicator counts down whenever the item is purchased. If the count reaches 0 the item will show a check mark indicating the item has been completed. Defaults to 0.
     */
    public int $count;
}