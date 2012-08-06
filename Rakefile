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
BUILD_DIR         = ROOT.join("www")
SOURCE_DIR        = ROOT.join("src")
ASSETS_SOURCE_DIR = SOURCE_DIR.join "assets"

namespace :build do
  
  desc "Clean www directory"
  task :clean do
    print "Cleaning www build... "
    FileUtils.mkdir_p BUILD_DIR
    Dir.glob File.join(BUILD_DIR, "**", "*") do |path|
      FileUtils.rm_rf path if File.exist? path
    end
    puts "done."
  end
  
  desc "Compile static assets"
  task :assets do
    print "Compiling static assets... "
    FileUtils.mkdir_p BUILD_DIR
    
    sprockets = Sprockets::Environment.new(SOURCE_DIR) do |env|
      env.logger = Logger.new( File.open( ROOT.join('log', 'sprockets.log'), "w") )
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
    puts "done."
  end


  desc "Build static HTML page"
  task :html do
    print "Building index.html... "
    if(File.exist?("src/index.haml"))
      
      require ROOT.join("lib", "mobox", "helpers")
      
      # Haml render scope
      render_scope = Object.new
      class << render_scope
        include Mobox::Helpers
      end
      
      FileUtils.mkdir_p BUILD_DIR
      if File.exist? File.join(BUILD_DIR, "index.html")
        File.delete File.join(BUILD_DIR, "index.html")
      end
      File.open(File.join(BUILD_DIR, "index.html"), "w") do |file|
        file.write Haml::Engine.new( File.open(File.join(SOURCE_DIR, "index.haml")).read ).render(render_scope)
        file.close
      end
    end
    puts "done."
  end
  
  desc "Copy images into build directory"
  task :images do
    print "Copying images... "
    FileUtils.mkdir_p BUILD_DIR
    
    img_build_dir = BUILD_DIR.join("images")
    img_src_dir = SOURCE_DIR.join("assets", "images")
    FileUtils.mkdir_p img_build_dir
    if Dir.exist? img_src_dir
      FileUtils.cp_r Dir[img_src_dir.join("*")], img_build_dir
    end
    puts "done."
  end
  
  task :all => [:clean, :html, :assets, :images]
  
end

desc "Build the application"
task :build => ["build:all"]

def prompt_with_choices input_string, choices, allow_input=false
  choices = [choices] unless choices.is_a? Array
  print "#{input_string} [#{choices.join('/')}]: "
  user_input = STDIN.gets.chomp.strip
  unless allow_input
    user_input = choices[0] unless choices.include? user_input
  end
  user_input
end


namespace :phonegap do
  
  desc "Create a phonegap app"
  task :create do
    
    project_platform = prompt_with_choices("Target platform?", %w[ios android])
    project_namespace = prompt_with_choices("Project namespace?", "com.example.projectname", true)
    project_name = prompt_with_choices("Project name?", "MyApp", true)
    build_command = "phonegap/lib/#{project_platform}/bin/create #{project_platform} #{project_namespace} #{project_name}"
    puts "running `#{build_command}`"
    system build_command
  end
  
  namespace :ios do
    
    desc "Build assets into ios App"
    task :build => ["build:all"] do
      print "Cleaning ios/www... "
      keep = %w[res/* config.xml]
      Dir.glob(ROOT.join('ios', 'www', '**', '*')).each do |filename|
        pattern_matches = keep.map{|match_string| filename =~ Regexp.new(ROOT.join('ios', 'www', match_string).to_s)}
        unless (pattern_matches[0] or not pattern_matches.reduce(&:==))
          FileUtils.remove_entry(filename) if File.exist?(filename) or Dir.exist?(filename)
        end
      end
      puts "done."
      
      print "Copying static assets to ios/www... "
      FileUtils.cp_r Dir[BUILD_DIR.join('*')], ROOT.join('ios', 'www')
      puts "done."
      
      print "Compiling ios app... "
      system "ios/cordova/debug > #{ROOT}/log/ios_build.log 2> #{ROOT}/log/ios_build.err.log"
      puts "done."
    end
    
  end
  
end

