<?php
use Illuminate\Support\Facades\Route;
use App\Http\Controllers\UserController;
use App\Http\Controllers\CardController;
use App\Http\Controllers\TagController;
use App\Http\Controllers\TradeController;
use App\Http\Controllers\MessageController;
use App\Http\Controllers\GradingController;
use App\Http\Controllers\FavoriteController;

// Controlador de card
Route::apiResource('cards', CardController::class)->names([
    'listCards'   => 'cards.listCards',   // Ver cartas disponibles
    'addCard'   => 'cards.addCard',   // Crear nueva carta
    'showCardsID'    => 'cards.showCardsID',    // Ver carta específica
    'updateCard'  => 'cards.updateCard',  // Actualizar carta (si fuera necesario)
    'destroyCard' => 'cards.destroyCard', // Eliminar carta (si fuera necesario)
]);

// Controlador de favorite
Route::apiResource('favorites', FavoriteController::class)->names([
    'listFavorites'   => 'favorites.listFavorites',   // Obtener todas las cartas favoritas
    'addToFavorites'   => 'favorites.addToFavorites',      // Agregar carta a favoritos
    'isFavorite'    => 'favorites.isFavorite',     // Verificar si una carta está en favoritos
    'removeFromFavorites' => 'favorites.removeFromFavorites',   // Eliminar carta de favoritos
]);

// Controlador de grading
Route::apiResource('gradings', GradingController::class)->names([
    'requestGrading'   => 'gradings.requestGrading',  // Solicitar gradeo de una carta
    'approveGrading'  => 'gradings.approveGrading',         // Aprobar gradeo de una carta
]);

// Controlador de mensaje
Route::apiResource('messages', MessageController::class)->names([
    'newMessage'        => 'messages.newMessage',        // Enviar un nuevo mensaje
    'getMessages'  => 'messages.getMessages',  // Obtener los mensajes de un usuario
]);

// Controlador de Tags
Route::apiResource('tags', TagController::class)->names([
    'listTags'   => 'tags.listTags',    // Obtener todas las etiquetas
    'addTag'   => 'tags.addTag',    // Crear una nueva etiqueta
    'showTagsID'    => 'tags.showTagsID',     // Obtener una etiqueta por ID
    'updateTag'  => 'tags.updateTag',   // Actualizar una etiqueta
    'destroyTag' => 'tags.destroyTag',  // Eliminar una etiqueta
]);

// Controlador de Trades
Route::apiResource('trades', TradeController::class)->names([
    'addTrade'     => 'trades.addTrade',     // Crear una solicitud de intercambio
    'acceptTrade'  => 'trades.acceptTrade',  // Aceptar un intercambio
    'rejectTrade'  => 'trades.rejectTrade',  // Rechazar un intercambio
]);

// Controlador de Users
Route::apiResource('users', UserController::class)->names([
    'listUsers'   => 'users.listUsers',    // Listar todos los usuarios (solo para administradores)
    'showUsersID'    => 'users.showUsersID',  // Ver un usuario por ID
    'updateUser'  => 'users.updateUser',   // Actualizar perfil
    'destroyUser' => 'users.destroyUser',  // Eliminar cuenta
]);






