#= require vendor/1140gs/css3-mediaqueries

#= require vendor/jquery
#= require vendor/gfx
#= require vendor/hammer
#= require vendor/jquery_transit

#= require vendor/spine/spine
#= require vendor/spine/route
#= require vendor/spine/manager
#= require vendor/spine/ajax
#= require vendor/spine/local
#= require vendor/spine/list
#= require vendor/spine/tabs
#= require vendor/spine/tmpl
#= require vendor/spine/touch
#= require vendor/spine/stage
#= require vendor/spine/panel

#= require ./app/index


jQuery ->
  app = new App el: $("body")
