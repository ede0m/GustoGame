$(document).ready(function(){

	var tileDict = {};
	var selectedColor = "#5773B2";
	var selectedTerrainKey = "o1";

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
		download(JSON.stringify(tileDict), fname, "application/json")
	});

	$('.terrain').click(function() {
		selectedColor = $(this).css("background-color");
		selectedTerrainKey = $(this).text();
		$('.terrain').css({"border-style":"none"});
		$(this).css({'border-style':'solid'});
	});

	$('#map').on('mousemove', '.tile', function(e){
		if (e.buttons === 1) {
	    	$(this).css({'background-color': selectedColor});
	    	var id = $(this).attr('id');
	    	tileDict[id] = selectedTerrainKey;
		}
	});

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
				tileDict[index] = selectedTerrainKey;
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

