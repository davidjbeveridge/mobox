# Mobox

Mobile app prototype in a box.

Mobox is intended to serve as a starting place for manufacturing a mobile app using a simple toolset: Haml, Sass, and CoffeeScript.
It lets you develop client-side web apps quickly without getting in the way, and takes care of the heavy lifting by assembling
everything w/ Sprockets


### What's included?

Mobox includes the tools that David Beveridge finds easiest to work with, including:

* Cordova (Phonegap) 2.0.0
* Haml for your homepage
* Sass for styles
  - 1140 Fluid Grid for layout
  - Compass for all that CSS3 goodness
* CoffeeScript
  - jQuery, because you can't live without it.
  - Spine.js for some structure in your JS
    + Spine Mobile, if you're into that
  - Haml-JS for client-side templates
  - GFX & jQuery Transit for hardware-accelerated animation
  - Hammer.js for multi-touch events
  
Mobox is also a build tool for your app, and can assemble the web, iOS, and Android versions.

### Prerequisites

For iOS apps, you'll need XCode and ios-sim. ios-sim can be installed with homebrew:

    brew install ios-sim

### Getting Started

    gem install mobox
    mobox new[my_app]
    cd my_app

Start the preview server with

    mobox server

and visit [http://localhost:3000](http://localhost:3000)

### Creating the mobile version

See below.  Builds will be output in `build/[platform name]`.

#### iOS

    mobox build:ios:create
    mobox build:ios

Running `mobox build:ios:create` will prompt you for a namespace and app name, which you must fill out in the correct format.

#### Android

    mobox build:android:create
    mobox build:android

Running `mobox build:android:create` will prompt you for a namespace and app name, which you must fill out in the correct format.

### Structure

You'll find everything you need to build your app in the `src` directory.

#### src/index.haml
The main page your app runs in.

#### src/assets/images
Put your images here.

#### src/assets/javascripts
Put your javascript/coffeescript here

#### src/assets/javascripts/app
Root for your Spine.js Application

#### src/assets/stylesheets
Put your sass/css/scss files here.

