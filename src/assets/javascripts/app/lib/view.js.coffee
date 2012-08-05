class App.View
  constructor: (context, locals) ->
    @[key] = value for key, value of context
    @[key] = value for key, value of locals
  
  config: (name) ->
    App.Config[name]
  
ViewMethods =
  view: (path, locals) ->
    JST["javascripts/app/views/#{path}"](new App.View(@, locals))

Spine.Controller.include ViewMethods
Panel.include ViewMethods
Stage.include ViewMethods