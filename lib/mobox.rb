Dir.glob( File.expand_path( File.join('..', 'vendor', '*', 'lib'), File.dirname(__FILE__) ) ) do |path|
  $:.unshift path
end


require "rubygems" # ruby1.9 doesn't "require" it though
require 'bundler'
require 'pathname'
require 'logger'
require 'compass'
require 'sprockets'
require 'sprockets-sass'
require 'sass'
require 'haml-sprockets'
require 'compass'
require 'compass-sprockets'
require 'yaml'
require 'json'

begin
  require 'fileutils'
rescue LoadError => e
  require 'ftools'
end

$:.unshift File.dirname(__FILE__)

require 'mobox/config'
require 'mobox/helpers'
require 'mobox/render_scope'
require 'mobox/server'

module Mobox
end