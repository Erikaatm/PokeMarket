<?php
use Illuminate\Support\Facades\Route;
use App\Http\Controllers\UserController;
use App\Http\Controllers\CardController;
use App\Http\Controllers\TagController;
use App\Http\Controllers\TradeController;
use App\Http\Controllers\MessageController;
use App\Http\Controllers\GradingController;
use App\Http\Controllers\FavoriteController;

Route::apiResource('cards', CardController::class);
Route::post('/trade', [TradeController::class, 'store']);
Route::post('/grade', [GradingController::class, 'requestGrading']);
