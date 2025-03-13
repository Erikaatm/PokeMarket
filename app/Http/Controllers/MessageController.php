<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use App\Models\Message;
use Illuminate\Support\Facades\Auth;

class MessageController extends Controller
{
    public function store(Request $request)
    {
        $message = Message::create([
            'from_user_id' => Auth::id(),
            'to_user_id' => $request->to_user_id,
            'message' => $request->message
        ]);

        return response()->json(['message' => 'Mensaje enviado.', 'data' => $message]);
    }

    public function getMessages($userId)
    {
        return response()->json(Message::where('from_user_id', Auth::id())->orWhere('to_user_id', Auth::id())->get());
    }
}
