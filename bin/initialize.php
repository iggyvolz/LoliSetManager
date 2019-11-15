<?php

use iggyvolz\lolisetmanager\Api;

require dirname(__DIR__)."/vendor/autoload.php";

(new class extends Api{
    public function realInitialize():void
    {
        parent::initialize();
    }
})::realInitialize();