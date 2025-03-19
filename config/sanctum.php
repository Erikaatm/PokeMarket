<?php

return [

    /*
    |--------------------------------------------------------------------------
    | Sanctum Authentication Guard
    |--------------------------------------------------------------------------
    |
    | Sanctum uses cookies to authenticate single page applications. You may
    | specify the guard that Sanctum should use for authenticating requests.
    |
    */

    'guard' => 'sanctum',

    /*
    |--------------------------------------------------------------------------
    | Sanctum Token Expiration
    |--------------------------------------------------------------------------
    |
    | You may specify the expiration of the issued tokens. If a value is
    | provided, tokens will expire after the specified number of minutes.
    |
    */

    'expiration' => 60, // En minutos

    /*
    |--------------------------------------------------------------------------
    | Sanctum Middleware
    |--------------------------------------------------------------------------
    |
    | Sanctum requires the "EnsureFrontendRequestsAreStateful" middleware for
    | proper functioning. This middleware makes sure your frontend can
    | authenticate using the session cookie, while also ensuring stateful.
    |
    */

    'middleware' => \Laravel\Sanctum\Http\Middleware\EnsureFrontendRequestsAreStateful::class,
];
