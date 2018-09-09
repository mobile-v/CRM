$(document).ready(function(){

	// --- COLLAPSE NAVBAR --- //
	(function navbarCollapse() {	
		var nbButton = $('button.navbar-toggle');
		var nbCollapse = $('div.navbar-collapse');
		nbButton.on('click', function(e) {
			e.preventDefault();
			nbCollapse.slideToggle(250);
		});
		$(window).resize(function() { 
			if ( $(window).width() > 768 ) {
				nbCollapse.removeAttr('style');
			};
		});	
	})();	
	
	
	// --- COLLAPSE TESTIMONIALS --- //
	(function toggleTestimonials() {
		var tmToggle = $('button.multiValued');
		var tmCollapse = $('div.testimonials-collapse');
		tmToggle.show();
		tmCollapse.hide();
		tmToggle.on('click', function(e) {
			e.preventDefault();
			var tmToggleSpan = $('span', this)
			var tmToggleText = tmToggleSpan.text() == "Свернуть" ? "Показть все" : "Свернуть";
			tmToggleSpan.text(tmToggleText);
			tmCollapse.slideToggle(500);
		});
	})();


	// --- SCROLL TO LINK --- //
	(function animatedScroll() {
		function scrollToSection(sectionId) {
			var destination = $(sectionId).offset().top;
			$('html:not(:animated),body:not(:animated)').animate({scrollTop: destination}, 600);
		};
		var links = $('ul.nav, li.navNewNo, div.nav2NewNo, div.go-top'); /*div.nav2*/
		links.on('click', 'a', function(e) {
			e.preventDefault();
			var sectionName = $(this).attr('href');
			scrollToSection(sectionName);
		});
		$(window).scroll(function() {
			var goTop = $('div.go-top');
			var showOn = $('#events');

		    
		        if ($(this).scrollTop() >= showOn.offset().top) {
		            goTop.fadeIn(200);
		        } else {
		            goTop.fadeOut(200);
		        };
		    

		});
	})();		


	// --- HYPHENATIONS --- //
	$.fn.hyphenate = function() {
		var all = "[абвгдеёжзийклмнопрстуфхцчшщъыьэюя]",
		glas = "[аеёиоуыэю\я]",
		sogl = "[бвгджзклмнпрстфхцчшщ]",
		zn = "[йъь]",
		shy = "\xAD",
		re = [];
		re[1] = new RegExp("("+zn+")("+all+all+")","ig");
		re[2] = new RegExp("("+glas+")("+glas+all+")","ig");
		re[3] = new RegExp("("+glas+sogl+")("+sogl+glas+")","ig");
		re[4] = new RegExp("("+sogl+glas+")("+sogl+glas+")","ig");
		re[5] = new RegExp("("+glas+sogl+")("+sogl+sogl+glas+")","ig");
		re[6] = new RegExp("("+glas+sogl+sogl+")("+sogl+sogl+glas+")","ig");
		return this.each(function() {
			var text = $(this).html();
			for (var i = 3; i < 7; ++i) {
				text = text.replace(re[i], "$1"+shy+"$2");
			}
			$(this).html(text);
		});
	};
});