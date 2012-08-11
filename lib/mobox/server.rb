require 'pathname'
require 'thin'
require 'rack'
require 'haml'

# Add Compass to Sass include path
Sass::Engine::DEFAULT_OPTIONS[:load_paths].tap do |paths|
  paths.push *Compass.sass_engine_options[:load_paths]
end


module Mobox
  class Server
    def initialize(path)
      root_path = self.root_path = Pathname(path).join('src')
      assets = self.assets
      map("/") {run IndexServer.new(root_path)}
      map("/stylesheets") {run assets}
      map("/javascripts") {run assets}
      map("/images") {run assets}
    end
    
    def run!
      Thin::Server.start('0.0.0.0', 3000, app)
    end
    
    def app
      @app ||= Rack::Builder.new
    end
    
    def map path, &block
      app.instance_eval do
        map path, &block
      end
    end
    
    def assets
      @assets ||= begin
        assets = Sprockets::Environment.new(root_path)
        www_source_root = root_path
        assets.context_class.instance_eval do
          include Mobox::Helpers
        end
        assets.context_class.const_set :WWW_SOURCE_ROOT, www_source_root
        assets.append_path(root_path.join('assets'))
        assets.append_path(root_path.join('assets', 'javascripts'))
        assets.append_path(root_path.join('assets', 'stylesheets'))
        assets.append_path(root_path.join('assets', 'images'))
        assets
      end
    end
    
    def index
      @index ||= IndexServer.new(root_path)
    end
    
    def root_path= path
      @root_path = Pathname(path.to_s)
    end

    def root_path
      @root_path
    end
    
    class IndexServer
      attr_accessor :source_path
      
      def initialize(path)
        self.source_path = Pathname(path)
      end
      
      def call(env)
        [200, headers, content]
      end
      
      def headers
        {'Content-Type'  => 'text/html', 'Cache-Control' => 'public, max-age=60'}
      end
      
      def render_scope
        Mobox::RenderScope.new(source_path)
      end
      
      def content
        Haml::Engine.new(File.open(source_path.join('index.haml'), File::RDONLY).read).render(render_scope)
      end
      
    end
    
  end
end