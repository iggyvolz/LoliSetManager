<?php
namespace iggyvolz\lolisetmanager;

class Lock
{
    private $fp;
    public function __construct(string $path)
    {
        touch($path);
        $this->fp = fopen($path, "r+");
    }
    public function __destruct()
    {
        fclose($this->fp);
    }
    public function lockRead():void
    {
        flock($this->fp, LOCK_SH);
    }
    public function lockWrite():void
    {
        flock($this->fp, LOCK_EX);
    }
    public function unlock():void
    {
        flock($this->fp, LOCK_UN);
    }
}