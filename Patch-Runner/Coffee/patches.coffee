#= require jqExtends.coffee
$ ->
	hub = $.connection.patchHub
	client = hub.client

	$('#recent-view').click -> showRecent()
	$('#recent-clear').click -> 
		$('#recent-modal .btn-stateful').disable()
		$.jStorage.flush()
		fillRecentList()

	$('#recent-run').click  -> 
		return false if $(@).isDisabled()
		$('#recent-modal').modal('hide')
		$.connection.patchHub.server.runPatch $('#recent-list').find('.success').attr('name')

	$('#patch-close').click -> 
		return false if $(@).isDisabled()
		$('#progress-modal').modal('hide')

	$('#patch-run-again').click ->
		return false if $(@).isDisabled()
		$.connection.patchHub.server.runPatch $('#progress-name').html()

	$.connection.hub.start().done -> $('#patch-run').click -> hub.server.runPatch $('#patch-name').val()

	client.patchStart = (name) -> showRunPatch name

	client.patchProgress = (percent) -> setProgress percent

	client.patchLog = (message, isList) -> addLog "<blockquote class=\"alert-info\" style=\"padding:5px;margin:3px;\"><p>#{message}</p></blockquote>", isList

	client.patchSuccess = (name, elapsed) -> 
		$('#progress-modal .btn-stateful').enable()
		addLog "<blockquote class=\"alert-success\" style=\"padding:5px;margin:3px;\"><h4>Success</h4>Time elapsed : <strong>#{elapsed}</strong></blockquote>", true
		saveLog name

	client.patchError = (name, method, line, message) ->
		$('#progress-modal .btn-stateful').enable()
		$('#progress-bar').removeClass('bar-success').addClass 'bar-danger'
		addLog "
			<blockquote class=\"alert-error\" style=\"padding:5px;margin:3px;\">
				<h4>Error</h4>
				<table class=\"table table-condensed alert-error\">
					<tr><td class=\"span1\">Method</td><td>#{method}</td></tr>
					<tr><td class=\"span1\">Line</td><td><strong>#{line}</td></tr>
					<tr><td class=\"span1\">Message</td><td><em style=\"font-size:12px;\">#{message}</em></td></tr>
				</table>
			</blockquote>", true
		saveLog name

	undefined

showRecent = ->
	$('#recent-modal .btn-stateful').disable()
	fillRecentList()
	$('#recent-modal').modal({keyboard: false, backdrop:'static'})

fillRecentList = ->
	logs = getLogs()
	logs.sort (a,b) -> sortBy('date', a, b, true)
	$('#recent-list')
		.empty()
		.append(->($("<tr name=\"#{log.name}\"><td>#{log.date}</td><td>#{log.name}</td></tr>") for log in logs))
		.on('click', 'tr', -> 
			$('#recent-modal .btn-stateful').enable()
			$(@).addClass('success').siblings().removeClass('success'))

showRunPatch = (name) ->
	$('#progress-name').html(name)
	$('#progress-log').empty()
	$('#progress-bar').removeClass('bar-danger').addClass('bar-success')
	setProgress('0%')
	$('#progress-modal .btn-stateful').disable()
	$('#progress-modal').modal({keyboard: false, backdrop:'static'})
	addLog("<blockquote class=\"alert-success\" style=\"padding:5px;margin:3px;\"><h4>Started</h4></blockquote>")

getLogs = ->
	$.jStorage.get('patch-logs') ? [{name:'[nothing run yet]', date:''}]

saveLog = (name) ->
	$log = $('#progress-log')
	logs = $.jStorage.get('patch-logs') ? []
	logs.push({name:name, date:getLogDate()})
	$.jStorage.set('patch-logs', logs)

setProgress=(percent) ->
	$('#progress-percent').html(percent)
	$('#progress-bar').css('width', percent)

addLog = (message, isList) ->
	entry = $('<div/>', {html:message})
	$log = $('#progress-log')
	$log.find('.alert-info').last().remove() if !isList
	$log.append(entry)
	$log.parent().scrollTop $log.height()

getLogDate = ->
	pad = (x) -> if x < 10 then '0' + x else '' + x
	date = new Date
	"#{pad date.getDate()}-#{pad date.getMonth() + 1}-#{date.getFullYear()} #{pad date.getHours()}:#{pad date.getMinutes()}:#{pad date.getSeconds()}"

sortBy = (key, a, b, r) ->
    r = if r then 1 else -1
    return -1*r if a[key] > b[key]
    return +1*r if a[key] < b[key]
    return 0

setEnabled= (el, state)->
	el.prop('disabled', !state)
	if state
		el.removeAttr('disabled')
	else
		el.attr('disabled','disabled')

