module Mobox
  module Helpers

    def config
      @config ||= Mobox::Config.new( File.expand_path("config.yml", WWW_SOURCE_DIR) )
    end

  end
end
