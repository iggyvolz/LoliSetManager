<?php
namespace iggyvolz\lolisetmanager\Sources;

use Iterator;
use iggyvolz\lolisetmanager\Source;
use iggyvolz\lolisetmanager\Collection;

class KoreanBuildsSource extends Source
{
    public function getSets():Iterator
    {
        $collection = new Collection();
        $champs = self::getJSON("https://api.koreanbuilds.net/champions?patchid=-1", [
            "Authorization: ".base64_encode("Basic kb-frontend T3M1ewuHj2QwsWB"),
            "Origin: https://koreanbuilds.net",
            "Referer: https://koreanbuilds.net/",
            "Accept: application/json",
        ]);
        var_dump($champs);
        yield "@koreanbuilds/latest" => $collection;
    }
}
