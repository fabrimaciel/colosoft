var SlideShow = function() 
{ 
    var slideShowID = arguments[0];
	
	this._slideShowID = slideShowID;
    this._slideImg = slideShowID + "_" +  this._slideImg;
    this._linkSlide = slideShowID + "_" +  this._linkSlide;
    this._titleSlide = slideShowID + "_" +  this._titleSlide;
    this._textSlide = slideShowID + "_" +  this._textSlide;
    this._playPause = slideShowID + "_" +  this._playPause;
    this._showTimer = slideShowID + "_" +  this._showTimer;
    this._itemsSlide = slideShowID + "_" +  this._itemsSlide;
    this._navigatorSlide = slideShowID + "_" +  this._navigatorSlide;
    this._pictureSlide = slideShowID + "_" +  this._pictureSlide;
    
    var itemsDiv = this.$(this._itemsSlide).childNodes;
    
    for (var i = 0; i < itemsDiv.length; i++) {
        
        this._images[i] = [itemsDiv[i].getAttribute("image"), 
                           itemsDiv[i].getAttribute("title"), 
                           itemsDiv[i].innerHTML,
                           itemsDiv[i].getAttribute("href")];
    }
};

SlideShow.prototype = 
{    
    _slideShowID : "",
    _images : [],
    // ID dos elementos que o sistema modifica
    // ID da imagem do slide
    _slideImg : 'slideImage',
    // ID do link do slide, ou seja, um elemento A
    _linkSlide : 'linkSlide',
    // ID do título do slide, uma div ou span por exemplo
    _titleSlide: 'titleSlide',
    // ID do título do slide, idêntico ao de cima
    _textSlide : 'textSlide',
    _itemsSlide : 'items',
    // ID da imagem de play|pause
    _playPause : 'playpause',
    // ID da div que mostra ou esconde as opções de customização de tempo
    // essa opção pode ser omitida do usuário, basta retirar o botão
    // settings da página, e como a div já vem com o display:none por
    // padrão o usuário não terá acesso a essas opções.
    _showTimer : 'showTimer',
	    	
    // variáveis do sistema
    // Daqui para baixo não é necessário alterar mais nada, aqui o sistema
    // cuidará de tudo
    _count : 0,
    _length : null,
    _timeOutID : null,
    _pause : false,
    _timer : 4,
	
    // função que inicia o slide e seta todas os parâmetros necessários
    start : function(){
	    with(this){
		    _preLoader();
		    _length = _images.length;
		    _work(); 
	    }
    },
	
    // faz o pré-carregamento das imagens
    _preLoader : function(){
	    for(x in this._images){
		    var image = new Image();
		    image.src = this._images[x][0];
	    }
    },
	
    // função principal que faz as checagens necessárias
    _work : function(){
	    with(this){
		    (_count == _length) ? _count = 0 : (_count < 0) ? _count = _length-1 : void(0);
		    var current = _images[_count];
		    _exchange(current);
		    if(!_pause){
			    (typeof(_timeOutID) == "number") ? clearTimeout(_timeOutID) : void(0);
			    
			    var slide = this;
			    _timeOutID = setTimeout(
					    function(){
						    slide.next();
						    fade(0,0,$(_slideImg));
					    }, (Number(_timer)*1000)
			    );
		    }
	    }
    },
	
    // função que altera os elementos da página, altere os IDs se necessário
    _exchange : function(img){
	    this.$(this._slideImg).src = img[0];
	    this.$(this._titleSlide).innerHTML = img[1];
	    this.$(this._textSlide).innerHTML = img[2];
	    this.$(this._linkSlide).href = img[3];	    
	    this.fade(0,100,this.$(this._slideImg));
    },
	
    // altera para o próximo slide ao clicar no botão Próximo
    next : function(){
	    with(this){
		    _count++;
		    _work();
	    }
    },
    // altera para a noticia escolhida
    position : function(val){
	    with(this){
		    _count=val;
		    _work();
	    }
    },
	
    // altera para o slide anterior ao clicar no botão correspondente
    previous : function(){
	    with(this){
		    _count--;
		    _work();
	    }
    },
	
    // pausa  a apresentação
    pause : function(){
		
	    if(this._pause){
		    this._pause = false;
					
	    }
	    else{
		    this._pause = true;
					
	    }
	    with(this){(typeof(_timeOutID) == 'number') ? clearTimeout(_timeOutID) : void(0); _work();}
    },
	
	
	
    // controla o tempo de troca de cada slide
    tControl : function(act){
	    with(this){
	    (act=="m")?((_timer==4)?void(0):_timer=_timer-1):((_timer==9)?void(0):_timer= _timer +1);
		    this.$(this._showTimer).innerHTML = _timer+"s";	
	    }
		
    },
	
    // altera a opacidade do elemento e suaviza a transição entre os slides
    fade : function (){
		
		return;
	    var type,signal;
	    var from 	= arguments[0];
	    var to		= arguments[1];
	    var el		= arguments[2];
		
	    (document.all) ? type = 'filter' : type = 'opacity';
	    (from>to) ? signal = '-' : signal= '+';
		
	    if(from >= to/2){
		    from = eval(from+signal+10);
	    }else{
		    from = eval(from+signal+5);
	    }
		
	    if(type=='opacity'){
		    try{el.style[type] = Number(from*0.01); }catch(e){}
	    }else{
		    try{el.style[type] = 'alpha(opacity='+from+')'; }catch(e){}
	    }
		
	    if(from != to){
		    setTimeout( function(){ slide.fade(from,to,slide.$(slide._slideImg)); } ,50);
	    }
    },
	
    // retorna o elemento solicitado através de seu ID
    $ : function(){
	    return document.getElementById(arguments[0]);	
    }
}