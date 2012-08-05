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



ROOT              = Pathname(File.dirname(__FILE__))
LOGGER            = Logger.new(STDOUT)
BUNDLES           = %w[ application.css application.js ]
BUILD_DIR         = ROOT.join("public")
SOURCE_DIR        = ROOT.join("src")
ASSETS_SOURCE_DIR = SOURCE_DIR.join "assets"

namespace :build do
  
  desc "Clean public directory"
  task :clean do
    Dir.glob File.join(BUILD_DIR, "**", "*") do |path|
      puts path
      FileUtils.rm_rf path if File.exist? path
    end
  end
  
  desc "Compile static assets"
  task :assets do
    sprockets = Sprockets::Environment.new(SOURCE_DIR) do |env|
      env.logger = Logger.new(STDOUT)
    end
    
    require ROOT.join("lib", "mobox", "helpers")
    sprockets.context_class.instance_eval do
      include Mobox::Helpers
    end

    # sprockets.append_path(File.join(SOURCE_DIR, 'assets'))
    sprockets.append_path(File.join(SOURCE_DIR, 'assets', 'javascripts'))
    # sprockets.append_path(File.join(SOURCE_DIR, 'assets', 'javascripts', 'app'))
    sprockets.append_path(File.join(SOURCE_DIR, 'assets', 'stylesheets'))

      BUNDLES.each do |bundle|
        assets = sprockets.find_asset(bundle)
        prefix, basename = assets.pathname.to_s.split('/')[-2..-1]
        FileUtils.mkpath BUILD_DIR.join(prefix)
        
        realname = assets.pathname.basename.to_s.split(".")[0..1].join(".")
        assets.write_to(BUILD_DIR.join(prefix, realname))
      end
  end


  desc "Build static HTML page"
  task :html do
    if(File.exist?("src/index.haml"))
      
      require ROOT.join("lib", "mobox", "helpers")
      
      # Haml render scope
      render_scope = Object.new
      class << render_scope
        include Mobox::Helpers
      end
      
      
      File.delete File.join(BUILD_DIR, "index.html") if File.exist? File.join(BUILD_DIR, "index.html")
      File.open(File.join(BUILD_DIR, "index.html"), "w") do |file|
        file.write Haml::Engine.new( File.open(File.join(SOURCE_DIR, "index.haml")).read ).render(render_scope)
        file.close
      end
    end
  end
  
  desc "Copy images into build directory"
  task :images do
    img_build_dir = File.join(BUILD_DIR, "images")
    img_src_dir = File.join(SOURCE_DIR, "images")
    FileUtils.mkdir_p img_build_dir
    if Dir.exist? img_src_dir
      FileUtils.cp_r img_src_dir, img_build_dir
    end
  end
  
  task :all => [:clean, :html, :assets, :images]
end

desc "Build the application"
task :build => ["build:all"]
