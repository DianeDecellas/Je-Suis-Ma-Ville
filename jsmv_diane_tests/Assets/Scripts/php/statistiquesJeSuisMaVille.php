<?php
$file=fopen("statistiques/je_suis_ma_ville.csv","a+");
fputs($file,$_POST["date"].",".$_POST["heure"].",".$_POST["nom"].",".$_POST["duree"].",".$_POST["note"]."\n");
fclose($file);
?>
