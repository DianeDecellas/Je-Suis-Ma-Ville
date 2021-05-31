<?php
$file=fopen("C:/Users/Diane/Documents/GitHub/Je-Suis-Ma-Ville/jsmv_diane_tests/Assets/Scripts/php/je_suis_ma_ville.csv","a+");
fputs($file,$_GET["date"].",".$_GET["heure"].",".$_GET["nom"].",".$_GET["duree"].",".$_GET["note"]."\n");
fclose($file);
?>
