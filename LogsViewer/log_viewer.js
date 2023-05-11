'use strict';
var logContent = null;
var turnIdx = 0;

function checkForm(){
    if($.trim($('#log-file').val()) == ''){
        alert('Choose log!');
        return false;
    }
    loadLog();
    return false;
}

function loadLog(){
    var logFile = $('#log-file').prop('files')[0];
    
    var reader = new FileReader();
    reader.onloadend = function (e){
        logContent = JSON.parse(e.target.result);
        console.log(logContent);

        turnIdx = 0;
        $('#header').html('Turn number <span id="turn">XX</span>/<span id="game-length">YY</span>, Player on turn <span id="player-id">ZZ</span>');
        loadGameResult( $('#game-summary'), logContent['Summary']['GameResult']);
        showLog();
    };
    reader.readAsText(logFile);
}

function clearViewer(){
    $('#viewer').html( $('#empty-viewer').html() );
}

function showLog(){
    if (logContent === null) return;
    clearViewer();
    
    var summary = logContent['Summary'];
    var detail = logContent['Detail'];
    if (detail.length == 0) return;

    var turnDetail = detail[turnIdx];

    $('#turn').text( turnDetail['Turn'] );
    $('#game-length').text( summary['GameLength'] );

    var playerOnTurn = turnDetail['PlayerIndex'];
    $('#player-id').text( playerOnTurn );
    
    var $botDiv = $('#viewer').find('.bot');
    var BotJson = turnDetail['BeginningOfTurn'];
    loadResolvedChainLink($botDiv, BotJson);

    var $playersCardsDiv = $('#viewer').find('.players-cards');
    var playersCards = turnDetail['PlayerCardsAfterBot'];
    for (let i = 0; i < playersCards.length; ++i) {
        var playerCards = playersCards[i];

        var $cardsContainer = $('<div>', {"class": "col"});
        $cardsContainer.append( $('#player-cards-template').html() );
        
        $cardsContainer.find('.player-number').text(i);
        if (i == playerOnTurn)
            $cardsContainer.find('.player').css('color', 'blue')

        addCards($cardsContainer.find('.hand'), playerCards['Hand']);
        addCards($cardsContainer.find('.stable'), playerCards['Stable']);
        addCards($cardsContainer.find('.upgrades'), playerCards['Upgrades']);
        addCards($cardsContainer.find('.downgrades'), playerCards['Downgrades']);

        $playersCardsDiv.append($cardsContainer);
    }

    var $cardsPlayingDiv = $('#viewer').find('.cards-playing');
    var cardsPlaying = turnDetail['CardPlaying'];
    for (let i = 0; i < cardsPlaying.length; ++i) {
        var stackResolve = cardsPlaying[i]['StackResolve'];

        var $stackContainer = $("<div>", {"class": "col"});

        $stackContainer.append( $('<div style="color: purple">').text(`${cardsPlaying[i]['CardToResolve']}; ${playerOnTurn} -> ${cardsPlaying[i]['TargetPlayer']}`));
        var depth = 0;
        for (let j = 0; j < stackResolve.length; ++j) {
            var actualCardOnStack = stackResolve[j];
            var playerReacted = actualCardOnStack['StackTypeLog'] == 1;

            if (playerReacted){
                var color = 'purple';
                depth += 1;
            }else{
                var color = 'green';
            }

            $stackContainer.append( $('<div style="position: relative; left: ' + depth * 20 + 'px; color: '+ color +'">')
                .text(actualCardOnStack['CardName'] + "; " + actualCardOnStack['OwningPlayer']));

            if (!playerReacted)
                depth -= 2;
        }


        var $cardEffect = $('<div/>'); 
        loadResolvedChainLink($cardEffect, cardsPlaying[i]['CardResolve']);
        $stackContainer.append($cardEffect);

        $cardsPlayingDiv.append($stackContainer);
    }

    var $endTurnDiv = $('#viewer').find('.end-turn');
    var endTurnJson = turnDetail['EndOfTurn'];
    loadResolvedChainLink($endTurnDiv, endTurnJson);
}

function loadResolvedChainLink($divContainer, jsonData){
    for (let i = 0; i < jsonData.length; ++i) {
        var $div = $("<div>", {"class": "col"});

        var effects = jsonData[i]['Effects'];
        for (let j = 0; j < effects.length; ++j) {
            var effect = effects[j];
            var $newEffect = $( $('#effect-template').html() );
            $newEffect.find('.owning-card').text( effect['OwningCard'] );
            $newEffect.find('.effect-name').text( effect['EffectType'].split('.').slice(-1) );
            
            var targets = effect['Targets'];
            var $targetsUl = $newEffect.find('.effect-targets');
            for (let x = 0; x < targets.length; ++x){
                $targetsUl.append( $("<li>").text(targets[x]) );
            }

            $div.append($newEffect);
        }
        $divContainer.append($div);
    }
}

function addCards($ulContainer, jsonData){
    for (let i = 0; i < jsonData.length; ++i) {
        $ulContainer.append( $("<li>").text( jsonData[i] ));
    }
}

function changeTurn(move){
    if (logContent === null) return;

    var newIdx = turnIdx + move;
    if (logContent['Detail'].length > newIdx && newIdx >= 0){
        turnIdx = newIdx;
        showLog();
    }
}

function previousTurn(){ changeTurn(-1); }
function nextTurn(){ changeTurn(+1); }

function loadGameResult($divContainer, gameResult){
    $divContainer.empty();
    $divContainer.append('<div class="row-name">Game Summary</div>');
    for (let i = 0; i < gameResult.length; ++i) {
        var playerInfo = gameResult[i];

        var $playerOverview = $( $('#player-overview').html() );
        $playerOverview.find('.id').text( playerInfo['PlayerId'] );
        $playerOverview.find('.place').text( i+1 );
        $playerOverview.find('.type').text( playerInfo['PlayerType'].split('.').slice(-1) );
        $playerOverview.find('.num-unicorns').text( playerInfo['NumUnicorns'] );
        $playerOverview.find('.sum-names').text( playerInfo['SumUnicornNames'] );
        $playerOverview.find('.avg-response').text( playerInfo['AverageResponse'] );
        
        $divContainer.append($playerOverview);
    }
}