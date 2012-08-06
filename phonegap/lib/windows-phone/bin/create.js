/*
       Licensed to the Apache Software Foundation (ASF) under one
       or more contributor license agreements.  See the NOTICE file
       distributed with this work for additional information
       regarding copyright ownership.  The ASF licenses this file
       to you under the Apache License, Version 2.0 (the
       "License"); you may not use this file except in compliance
       with the License.  You may obtain a copy of the License at

         http://www.apache.org/licenses/LICENSE-2.0

       Unless required by applicable law or agreed to in writing,
       software distributed under the License is distributed on an
       "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
       KIND, either express or implied.  See the License for the
       specific language governing permissions and limitations
       under the License.
*/

/*
 * create a cordova/wp7 project
 *
 * USAGE
 *  ./create [path package activity]
 */

function Usage()
{
  WScript.Echo("Usage: create [PATH]"); // [PACKAGE] [ACTIVITY]");
  WScript.Echo("Creates a new cordova/wp7 project.");
}

var ForReading = 1, ForWriting = 2, ForAppending = 8;
var TristateUseDefault = -2, TristateTrue = -1, TristateFalse = 0;

function read(filename) {
    //WScript.Echo('Reading in ' + filename);
    var fso=WScript.CreateObject("Scripting.FileSystemObject");
    var f=fso.OpenTextFile(filename, 1,2);
    var s=f.ReadAll();
    f.Close();
    return s;
}

function write(filename, contents) {
    var fso=WScript.CreateObject("Scripting.FileSystemObject");
    var f=fso.OpenTextFile(filename, ForWriting, TristateTrue);
    f.Write(contents);
    f.Close();
}
function replaceInFile(filename, regexp, replacement) {
    write(filename,read(filename).replace(regexp,replacement));
}
function exec(s, output) {
    WScript.Echo('Executing::' + s);
    var o=shell.Exec(s);
    while (o.Status == 0) {
        WScript.Sleep(100);
    }
    WScript.Echo(o.StdErr.ReadAll());
    WScript.Echo("Command exited with code " + o.Status);
}

function fork(s) {
    WScript.Echo('Executing ' + s);
    var o=shell.Exec(s);
    while (o.Status != 1) {
        WScript.Sleep(100);
    }
    WScript.Echo(o.StdOut.ReadAll());
    WScript.Echo(o.StdErr.ReadAll());
    WScript.Echo("Command exited with code " + o.Status);
}

function genGuid()
{
    var TypeLib = WScript.CreateObject("Scriptlet.TypeLib");
    strGuid = TypeLib.Guid.split("}")[0]; // there is extra crap after the } that is causing file streams to break, probably an EOF ... 
    strGuid = strGuid.replace(/[\{\}]/g,""); 
    return strGuid;
}

var args = WScript.Arguments,
    PROJECT_PATH="..\\example\\", 
    PACKAGE="org.apache.cordova.example", 
    ACTIVITY="cordovaExample",
    shell=WScript.CreateObject("WScript.Shell");
    
// working dir
var ROOT = WScript.ScriptFullName.split('\\bin\\create.js').join('');

if (args.Count() > 0) 
{
    PROJECT_PATH = args(0);
    if(PROJECT_PATH.indexOf("--help") > -1 ||
       PROJECT_PATH.indexOf("/?") > -1 ) 
    {
       Usage();
       WScript.Quit(1);
    }

    if(args.Count() > 1)
    {
      PACKAGE=args(1);
    }

    if(args.Count() > 2)
    {
      ACTIVITY=args(2);
    }

}

// WScript.Echo("ROOT = " + ROOT);
// WScript.Echo('PROJECT_PATH ' + PROJECT_PATH);
// WScript.Echo('PACKAGE ' + PACKAGE);
// WScript.Echo('ACTIVITY ' + ACTIVITY);

var PACKAGE_AS_PATH=PACKAGE.replace(/\./g, '\\');
WScript.Echo("Package as path: " + PACKAGE_AS_PATH);

var newProjGuid = genGuid();

// Copy the template source files to the new destination
exec('cmd /c xcopy templates\\full ' + PROJECT_PATH + ' /S /Y');
// replace the guid in the AppManifest
replaceInFile(PROJECT_PATH + "\\Properties\\WMAppManifest.xml","$guid1$",newProjGuid);
// replace safe-project-name in AppManifest
replaceInFile(PROJECT_PATH + "\\Properties\\WMAppManifest.xml",/\$safeprojectname\$/g,ACTIVITY);

WScript.Echo("Generated project : " + PROJECT_PATH + ACTIVITY);

// TODO: Name the project according to the arguments
// update the solution to include the new project by name
// version BS
// index.html title set to project name ?






// var ACTIVITY_PATH=PROJECT_PATH+'\\src\\'+PACKAGE_AS_PATH+'\\'+ACTIVITY+'.java';
// var MANIFEST_PATH=PROJECT_PATH+'\\AndroidManifest.xml';
// var TARGET=shell.Exec('android.bat list targets').StdOut.ReadAll().match(/id:\s([0-9]).*/)[1];
// var VERSION=read('VERSION').replace(/\r\n/,'').replace(/\n/,'');

// WScript.Echo("Project path: " + PROJECT_PATH);
// WScript.Echo("Package: " + PACKAGE);
// WScript.Echo("Activity: " + ACTIVITY);
// WScript.Echo("Package as path: " + PACKAGE_AS_PATH);
// WScript.Echo("Activity path: " + ACTIVITY_PATH);
// WScript.Echo("Manifest path: " + MANIFEST_PATH);
// WScript.Echo("Cordova version: " + VERSION);
