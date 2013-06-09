
$.fn.disable=-> setState $(@), true
$.fn.enable =-> setState $(@), false
$.fn.isDisabled =-> $(@).hasClass 'disabled'

setState=($el, state) ->
	$el.each ->
		$(@).prop('disabled', state) if $(@).is 'button, input'
		if state then $(@).addClass('disabled') else $(@).removeClass('disabled')

	$('body').on('click', 'a.disabled', -> false)

