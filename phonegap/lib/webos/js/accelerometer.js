/*
 *
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 *
*/

/*
 * This class provides access to device accelerometer data.
 * @constructor
 */
function Accelerometer() {
	/*
	 * The last known acceleration.
	 */
	this.lastAcceleration = null;
};

/*
 * Tells WebOS to put higher priority on accelerometer resolution. Also relaxes the internal garbage collection events.
 * @param {Boolean} state
 * Dependencies: Mojo.windowProperties
 * Example:
 * 		navigator.accelerometer.setFastAccelerometer(true)
 */
Accelerometer.prototype.setFastAccelerometer = function(state) {
	navigator.windowProperties.fastAccelerometer = state;
	navigator.window.setWindowProperties();
};

/*
 * Asynchronously aquires the current acceleration.
 * @param {Function} successCallback The function to call when the acceleration
 * data is available
 * @param {Function} errorCallback The function to call when there is an error 
 * getting the acceleration data.
 * @param {AccelerationOptions} options The options for getting the accelerometer data
 * such as timeout.
 */

Accelerometer.prototype.getCurrentAcceleration = function(successCallback, errorCallback, options) {

    var referenceTime = 0;
    if (this.lastAcceleration)
        referenceTime = this.lastAcceleration.timestamp;
    else
        this.start();
 
    var timeout = 20000;
    var interval = 500;
    if (typeof(options) == 'object' && options.interval)
        interval = options.interval;
 
    if (typeof(successCallback) != 'function')
        successCallback = function() {};
    if (typeof(errorCallback) != 'function')
        errorCallback = function() {};
 
    var dis = this;
    var delay = 0;
    var timer = setInterval(function() {
        delay += interval;
 
		//if we have a new acceleration, call success and cancel the timer
        if (typeof(dis.lastAcceleration) == 'object' && dis.lastAcceleration != null && dis.lastAcceleration.timestamp > referenceTime) {
            successCallback(dis.lastAcceleration);
            clearInterval(timer);
        } else if (delay >= timeout) { //else if timeout has occured then call error and cancel the timer
            errorCallback();
            clearInterval(timer);
        }
		//else the interval gets called again
    }, interval);
};


/*
 * Asynchronously aquires the acceleration repeatedly at a given interval.
 * @param {Function} successCallback The function to call each time the acceleration
 * data is available
 * @param {Function} errorCallback The function to call when there is an error 
 * getting the acceleration data.
 * @param {AccelerationOptions} options The options for getting the accelerometer data
 * such as timeout.
 */

Accelerometer.prototype.watchAcceleration = function(successCallback, errorCallback, options) {
	this.getCurrentAcceleration(successCallback, errorCallback, options);
	// TODO: add the interval id to a list so we can clear all watches
 	var frequency = (options != undefined)? options.frequency : 10000;
	var that = this;
	return setInterval(function() {
		that.getCurrentAcceleration(successCallback, errorCallback, options);
	}, frequency);
};

/*
 * Clears the specified accelerometer watch.
 * @param {String} watchId The ID of the watch returned from #watchAcceleration.
 */
Accelerometer.prototype.clearWatch = function(watchId) {
	clearInterval(watchId);
};

/*
 * Starts the native acceleration listener.
 */

Accelerometer.prototype.start = function() {
	var that = this;
	//Mojo.Event.listen(document, "acceleration", function(event) {
	document.addEventListener("acceleration", function(event) {
		var accel = new Acceleration(event.accelX, event.accelY, event.accelZ);
		that.lastAcceleration = accel;
	});
};

if (typeof navigator.accelerometer == "undefined") navigator.accelerometer = new Accelerometer();

