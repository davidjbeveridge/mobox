class App.Home extends Spine.Controller

  constructor: ->
    super
    @active @render
  
  render: =>
    @html @view('home/index')
    $('#homescreen img').on 'tap click', (e) ->
      $(e.currentTarget).toggleClass('flipped')