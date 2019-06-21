$(document).ready(function(){

	var tileDict = {};
	var selectedColor = "#5773B2";
	var selectedGroundObjectColor = "black"
	var selectedTerrainKey = "o1";
	var selectedSittingObjectKey = "null";
	var selectedRegionName = "GustoMap";

	$('#init').click(function() {
		var width = parseInt($('#width').val());
		var height = parseInt($('#height').val());
		var multX = parseInt($('#multX').val());
		var multY = parseInt($('#multY').val());
		var tileSize = parseInt($('#tileSize').val());
		var nTiles = ((width * multX) / tileSize) * ((height * multY) / tileSize);
		BuildMap(width, height, multX, multY, tileSize);
	});

	$('#exportjson').click(function(){
		var fname = $('#filename').val(); 
		download(JSON.stringify(tileDict), fname, "application/json");
		download("var mapdata = " + JSON.stringify(tileDict), "mapdata.js", "text/javascript");
	});

	$('#loadjson').click(function(){
		var loadedMap = JSON.parse(JSON.stringify(mapdata));
		LoadMap(mapdata, parseInt($('#width').val()), parseInt($('#height').val()), parseInt($('#multX').val()), parseInt($('#multY').val()), parseInt($('#tileSize').val()))
	});

	$('#newRegion').click(function(){
		$('.selectRegion').prop('checked', false);
		$('#regionList').append('<li><input type="radio" class="selectRegion" checked><input type="text" value="GustoMap" class="region"></li>')
	});

	$('#regionList').on('click', '.selectRegion', function(e){
		$('.selectRegion').prop('checked', false);
		$(this).prop('checked', true);
		selectedRegionName = $(this).siblings('.region').val();
	});

	$('.terrain').click(function() {
		selectedColor = $(this).css("background-color");
		selectedTerrainKey = $(this).text();
		$('.terrain').css({"border-style":"none"});
		$(this).css({'border-style':'solid'});
	});

	$('.groundObject').click(function() {
		selectedGroundObjectColor = $(this).css("background-color");
		selectedSittingObjectKey = $(this).text();
		$('.groundObject').css({"border-style":"none"});
		$(this).css({'border-style':'solid'});
	});

	// user setting the map
	$('#map').on('mousemove', '.tile', function(e){
		
		var id = $(this).attr('id');

		if (e.buttons === 1) {
	    	$(this).css({'background-color': selectedColor});
	    	var entry = {
	    		terrainPiece:selectedTerrainKey,
	    		sittingObject:selectedSittingObjectKey,
	    		regionName:selectedRegionName
	    	};
	    	tileDict[id] = entry;

	    	$(this).text(selectedRegionName.substring(0, 2));
	    	$(this).css("fontSize", 10);
	    	$(this).css("color", selectedGroundObjectColor)
		}
	});

	function LoadMap(mapdata, width, height, multX, multY, tileSize) {

		tileDict = mapdata;
		$('#map').empty();

		var cols = Math.floor((width / tileSize) * multX);
		var rows = Math.floor((height / tileSize) * multY);

		var displayTileSizeX = 16;
		var displayTileSizeY = 16;

		document.getElementById('map').style.setProperty('--ncols', cols.toString());
		document.getElementById('map').style.setProperty('--nrows', rows.toString());
		document.getElementById('map').style.setProperty('--tileX', displayTileSizeX.toString() + "px");
		document.getElementById('map').style.setProperty('--tileY', displayTileSizeY.toString() + "px");


		var index = 0;
		for (var i = 0; i < rows; i++) {
			for (var j = 0; j < cols; j++) {
				$('#map').append('<div class="tile" id="'+ index + '" style="width:' + displayTileSizeX + 'px;height:' + displayTileSizeY + 'px;"></div>');
				var entry = tileDict[index];
				
				if (!entry.hasOwnProperty('sittingObject')) {
					var entryTer = tileDict[index]['terrainPiece'];
					var entryReg = tileDict[index]['regionName'];
					entry= {};
					entry['regionName'] = entryReg;
					entry['terrainPiece'] = entryTer;
					entry['sittingObject'] = selectedSittingObjectKey;
				}

				$('#' + index).css({'background-color': $('#' + entry['terrainPiece']).css("background-color")});
				$('#' + index).text(entry['regionName'].substring(0, 2));
				$('#' + index).css({'color':$('#' + entry['sittingObject']).css("background-color")});
		    	$('#' + index).css("fontSize", 10);
		    	tileDict[index] = entry;
				index++;
			}
		}

		$('#nTiles').text((cols * rows));
		$('#cols').text(cols);
		$('#rows').text(rows);

	}

	// itital building of a new map
	function BuildMap(width, height, multX, multY, tileSize) {
		
		tileDict = {};
		$('#map').empty();

		var cols = Math.floor((width / tileSize) * multX);
		var rows = Math.floor((height / tileSize) * multY);

		var displayTileSizeX = 16;
		var displayTileSizeY = 16;

		document.getElementById('map').style.setProperty('--ncols', cols.toString());
		document.getElementById('map').style.setProperty('--nrows', rows.toString());
		document.getElementById('map').style.setProperty('--tileX', displayTileSizeX.toString() + "px");
		document.getElementById('map').style.setProperty('--tileY', displayTileSizeY.toString() + "px");

		var index = 0;
		for (var i = 0; i < rows; i++) {
			for (var j = 0; j < cols; j++) {
				$('#map').append('<div class="tile" id="'+ index + '" style="width:' + displayTileSizeX + 'px;height:' + displayTileSizeY + 'px;"></div>');
		    	var entry = {
		    		terrainPiece:selectedTerrainKey,
		    		sittingObject:selectedSittingObjectKey,
		    		regionName:selectedRegionName
		    	};
	    		tileDict[index] = entry;
				index++;
			}
		}

		$('#nTiles').text((cols * rows));
		$('#cols').text(cols);
		$('#rows').text(rows);
		
		//TODO - make tiles and map not draggable
		
		// color the origin
		var origin = Math.floor(((cols * rows) / 2) - (cols/2));
		$('#'+origin).css('background-color', 'red');
	}

	// Function to download data to a file
	function download(data, filename, type) {
	    var file = new Blob([data], {type: type});
	    if (window.navigator.msSaveOrOpenBlob) // IE10+
	        window.navigator.msSaveOrOpenBlob(file, filename);
	    else { // Others
	        var a = document.createElement("a"),
	                url = URL.createObjectURL(file);
	        a.href = url;
	        a.download = filename;
	        document.body.appendChild(a);
	        a.click();
	        setTimeout(function() {
	            document.body.removeChild(a);
	            window.URL.revokeObjectURL(url);  
	        }, 0); 
	    }
	}

});

