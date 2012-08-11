module Mobox
  class RenderScope < Object
    def initialize(source_root)
      RenderScope.const_set :WWW_SOURCE_ROOT, source_root
    end
    
    include Mobox::Helpers
  end
end