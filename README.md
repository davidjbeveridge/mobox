# Mobox

Mobile app prototype in a box.

Mobox is intented to serve as a starting place for manufacturing a mobile app using a simple toolset: Haml, Sass, and CoffeeScript.
It lets you develop client-side web apps quickly without getting in the way, and takes care of the hard stuff like packaging everything
together for you.

**prototype: (n)**

1. one of the first units manufactured of a product, which is tested so that the design can be changed if necessary before the product is manufactured commercially


### What's included?

Mobox is built using the tools that David Beveridge finds easiest to work with, including:

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
  


### Getting Started

    git clone [repo url] my_app
    cd my_app
    rm -rf .git

#### Structure

    ├── src
    │   ├── assets
    │   │   ├── images
    │   │   ├── javascripts
    │   │   │   ├── app
    │   │   │   │   ├── controllers
    │   │   │   │   ├── helpers
    │   │   │   │   ├── lib
    │   │   │   │   ├── models
    │   │   │   │   └── views
    |   |   |   └──application.sass
    │   │   └── stylesheets
    │   │       └── vendor
    │   │       |   ├── 1140gs
    │   │       |   └── spine_mobile
    |   |       └──application.sass
    │   └── index.haml
    └── tmp
        └── pids



# TODO

* Finish toolset
* Update README