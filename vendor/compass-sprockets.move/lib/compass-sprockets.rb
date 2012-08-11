require "sprockets"
require "sprockets/compressors"
require "sprockets/static_compiler"

## override a compass function that appears to contain a typo
module Compass
  module Configuration
    def self.add_configuration_property(name, comment = nil, &default)
      ATTRIBUTES << name
      if comment.is_a?(String)
        unless comment[0..0] == "#"
          comment = "# #{comment}"
        end
        unless comment[-1..-1] == "\n"
          comment = comment + "\n"
        end
        Data.class_eval <<-COMMENT
          def comment_for_#{name}
            #{comment.inspect}
          end
        COMMENT
      end
      Data.send(:define_method, :"default_#{name}", &default) if default
      Data.inherited_accessor(name)
      if name.to_s =~ /dir|path/
        Data.strip_trailing_separator(name)
      end
    end
  end
end

require "compass/sprockets"
