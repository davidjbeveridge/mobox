require 'yaml'
require 'json'

module Mobox
  class Config

    def initialize(file)
      @attributes = YAML.load(File.open( file ).read)
    end

    def [] attr_name
      @attributes[attr_name.to_s]
    end

    def []= attr_name, value
      @attributes[attr_name.to_s] = value
    end

    def method_missing name, *args, &block
      if @attributes.has_key? name.to_s
        @attributes[name.to_s]
      else
        super
      end
    end

    def respond_to? name
      @attributes.has_key?(name.to_s) or super
    end
    
    def to_json
      @attributes.to_json
    end

  end
end
