require "compass/sprockets/version"

module Compass
  module Sprockets
    extend self

    Compass::Frameworks.register("sprockets", :path => "#{File.dirname(__FILE__)}/..")

    # add in the configuration file
    Compass::Configuration.add_configuration_property(:javascripts_src_dir, "Location of JavaScript source files for use with Sprockets") do
      "javascripts" + "/src"
    end

    Compass::Configuration.add_configuration_property(:javascript_compressor, "JavaScript compressor for use with Sprockets") do
      :uglifier
    end

    Compass::Configuration.add_configuration_property(:compress_javascript, "Determines if JavaScript should be compressed") do
      if environment == :production
        true
      else
        false
      end
    end
    
    # register a custom watch
    Compass.configuration.watch Compass.configuration.javascripts_src_dir + "/**/*" do |project_dir, relative_path|
      puts project_dir
      puts relative_path
      if File.exists?(File.join(project_dir, relative_path))
        # configure environment
        environment = Sprockets::Environment.new(project_dir)
        environment.append_path(Compass.configuration.javascripts_src_dir)

        # add compressor if defined
        if Compass.configuration.compress_javascript
          environment.js_compressor = Sprockets::LazyCompressor.new {
            Sprockets::Compressors.registered_js_compressor(Compass.configuration.javascript_compressor)
          }
        end
        
        # configure the static compiler
        compiler = Sprockets::StaticCompiler.new(
          environment,
          Compass.configuration.javascripts_dir,
          [/^([\w-]+\/)?[^_]\w+\.(js|coffee)$/],
          :manifest_path => Compass.configuration.javascripts_dir,
          :digest => false,
          :manifest => false,
          :zip_files => /a^/
          )
        compiler.compile
      end
    end
  end
end