<?php

use iggyvolz\lolisetmanager\Api;

require dirname(__DIR__)."/vendor/autoload.php";

if(isset($_GET["method"]))
{
  $method=$_GET["method"];
}
else
{
    http_response_code(404);
    die("Method not found.");
}
if(method_exists(Api::class,$method))
{
  $f=new ReflectionMethod(Api::class,$method);
  if(!$f->isPublic())
  {
    http_response_code(404);
    die("Method not found.");
  }
  $args=[];
  foreach($f->getParameters() as $param)
  {
    if(isset($_GET[$param->name]))
    {
      $args[]=$_GET[$param->name];
    }
    elseif($param->isDefaultValueAvailable())
    {
      $args[]=$param->getDefaultValue();
    }
    else
    {
      http_response_code(400);
      die("Requires param ".$param->name);
    }
  }
  $data=Api::$method(...$args);
  $d=json_encode($data);
  echo $d;
}
else
{
    http_response_code(404);
    die("Method not found.");
}