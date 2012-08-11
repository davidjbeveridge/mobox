# -*- encoding: utf-8 -*-
$:.push File.expand_path("../lib", __FILE__)
require 'mobox/version'

Gem::Specification.new do |gem|

  gem.authors       = ["David Beveridge"]
  gem.email         = ["davidjbeveridge@gmail.com"]
  gem.description   = %q{Mobox builds mobile app quickly using Haml, SASS, CoffeeScript, and Phonegap, to name a few.}
  gem.summary       = %q{Mobile apps in a box}
  gem.homepage      = "https://github.com/davidjbeveridge/mobox"

  gem.files         = `git ls-files`.split($\)
  gem.executables   = gem.files.grep(%r{^bin/}).map{ |f| File.basename(f) }
  gem.test_files    = gem.files.grep(%r{^(test|spec|features)/})
  gem.name          = "mobox"
  gem.require_paths = ["lib"]
  gem.version       = Mobox::VERSION
  
  gem.add_dependency "rack"
  gem.add_dependency "thor"
  gem.add_dependency "sprockets"
  gem.add_dependency "sprockets-sass"
  gem.add_dependency "thin"
  gem.add_dependency "coffee-script"
  gem.add_dependency "sass", "~>3.2.0.alpha.277"
  gem.add_dependency "compass"
  gem.add_dependency "haml"
  gem.add_dependency "json"

end
