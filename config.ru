require 'rubygems'
require 'bundler'
Bundler.require if File.exist? "Gemfile"
require 'pathname'
require 'logger'
require 'compass'
require 'sprockets'
require 'sprockets-sass'
require 'haml-sprockets'
require 'sass'
require 'compass'
require 'compass-sprockets'
require 'yaml'
require 'json'
begin
  require 'fileutils'
rescue LoadError => e
  require 'ftools'
end

Sass::Engine::DEFAULT_OPTIONS[:load_paths].tap do |paths|
  paths.push *Compass.sass_engine_options[:load_paths]
end

ROOT              = Pathname(File.dirname(__FILE__))
LOGGER            = Logger.new(STDOUT)
BUNDLES           = %w[ application.css application.js ]
BUILD_DIR         = ROOT.join("public")
SOURCE_DIR        = ROOT.join("src")
ASSETS_SOURCE_DIR = SOURCE_DIR.join "assets"

assets = Sprockets::Environment.new(SOURCE_DIR) do |env|
  env.logger = Logger.new(STDOUT)
end

# Sprockets render scope
require ROOT.join("lib", "mobox", "helpers")
assets.context_class.instance_eval do
  include Mobox::Helpers
end

# Haml render scope
render_scope = Object.new
class << render_scope
  include Mobox::Helpers
end

assets.append_path(File.join(SOURCE_DIR, 'assets'))
assets.append_path(File.join(SOURCE_DIR, 'assets', 'javascripts'))
assets.append_path(File.join(SOURCE_DIR, 'assets', 'stylesheets'))
assets.append_path(File.join(SOURCE_DIR, 'assets', 'images'))

map "/javascripts" do
  run assets
end

map "/stylesheets" do
  run assets
end

map "/images" do
  run assets
end

map "/" do
  run lambda { |env|
    [
      200, 
      {
        'Content-Type'  => 'text/html', 
        'Cache-Control' => 'public, max-age=60'
      },
      Haml::Engine.new(File.open(SOURCE_DIR.join('index.haml'), File::RDONLY).read).render(render_scope)
    ]
  }
end