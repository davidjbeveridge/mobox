@App = class App extends Spine.Controller
  constructor: ->
    super
    @append @home = new App.Home
    @home.active()

