<?php
namespace iggyvolz\lolisetmanager;

interface ISource
{
    public function getSets():\iterator;
}