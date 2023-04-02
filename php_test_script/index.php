<?php
//test script for trackevent_test

ini_set('display_errors', 'on'); error_reporting(E_ALL|E_STRICT);

$postData = file_get_contents('php://input');
$jsonString = urldecode($postData);
file_put_contents("output.txt", $jsonString);

if (isset($_SERVER["HTTP_ORIGIN"]))
    header("Access-Control-Allow-Origin: {$_SERVER['HTTP_ORIGIN']}");
else
    header("Access-Control-Allow-Origin: *");

header("Access-Control-Allow-Credentials: true");

if($_SERVER["REQUEST_METHOD"] == "OPTIONS")
{
    if (isset($_SERVER["HTTP_ACCESS_CONTROL_REQUEST_METHOD"]))
        header("Access-Control-Allow-Methods: POST, GET, OPTIONS, DELETE, PUT");

    if (isset($_SERVER["HTTP_ACCESS_CONTROL_REQUEST_HEADERS"]))
        header("Access-Control-Allow-Headers: {$_SERVER['HTTP_ACCESS_CONTROL_REQUEST_HEADERS']}");

	exit(0); //CORS preflight response
}

header("HTTP/1.1 200 OK");
//header("HTTP/1.1 418 I'm a teapot"); //test server error