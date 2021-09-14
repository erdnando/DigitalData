//
// Dynamsoft JavaScript Library for Basic Initiation of Dynamic Web TWAIN
// More info on DWT: http://www.dynamsoft.com/Products/WebTWAIN_Overview.aspx
//
// Copyright 2016, Dynamsoft Corporation 
// Author: Dynamsoft Team
// Version: 11.3
//
/// <reference path="dynamsoft.webtwain.initiate.js" />
var Dynamsoft = Dynamsoft || { WebTwainEnv: {} };

Dynamsoft.WebTwainEnv.AutoLoad = true;
///
Dynamsoft.WebTwainEnv.Containers = [{ ContainerId: 'dwtcontrolContainer', Width: 400, Height: 550 }];
///
Dynamsoft.WebTwainEnv.ProductKey = '*****' //'19D89F144311D43AE5FED4AB61BE2DDD8B9AED83F81E731BAB371AFB411DC51BC910371ADEC4DC09A99A9413EB120A062B1CCFAD864A105794F0D1188F862EC032DDF1203D4AC61BE798C4D15F196F845593B5A86E19D3898F19D7EC817268D565241167EB5C59B306D812CD9610EB010D0B27820DF6AEC63A5E391AA9467F163149D146F6C6DFEF';
///
Dynamsoft.WebTwainEnv.Trial = true;
///
Dynamsoft.WebTwainEnv.ActiveXInstallWithCAB = false;
///
Dynamsoft.WebTwainEnv.Debug = true; // only for debugger output
///
Dynamsoft.WebTwainEnv.ResourcesPath = '../Resources';

/// All callbacks are defined in the dynamsoft.webtwain.install.js file, you can customize them.

// Dynamsoft.WebTwainEnv.RegisterEvent('OnWebTwainReady', function(){
// 		// webtwain has been inited
// });

