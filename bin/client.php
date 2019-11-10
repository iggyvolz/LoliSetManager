<?php
define("API_URL", "http://127.0.0.1:8080/");
define("LEAGUE_BASE_PATH", "/home/katie/.local/share/leagueoflegends/LOL");
define("COLLECTION", "@championgg/latest");
function api(string $method, array $args)
{
    $url=API_URL."?method=$method";
    foreach($args as $name=>$value) {
        $url.="&$name=$value";
    }
    $ch = curl_init();
    curl_setopt($ch, CURLOPT_URL, $url);
    curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
    $res = json_decode(curl_exec($ch), true);
    curl_close($ch);
    return $res;
}

$collection = api("getCollection", ["name"=>COLLECTION]);
foreach($collection as $champ=>$sets) {
    foreach($sets as $setname) {
        $set = api("getItemSet", ["name" => $setname]);
        if($champ === "Global") {
            $dir = LEAGUE_BASE_PATH . "/Config/Global/Recommended";
        } else {
            $dir = LEAGUE_BASE_PATH . "/Config/Champions/$champ/Recommended";
        }
        if(!is_dir($dir)) {
            mkdir($dir, 0777, true);
        }
        file_put_contents("$dir/".urlencode($setname).".json", json_encode($set));
    }
}