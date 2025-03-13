<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use App\Models\Trade;
use Illuminate\Support\Facades\Auth;

class TradeController extends Controller
{
    public function store(Request $request)
    {
        $trade = Trade::create([
            'from_user_id' => Auth::id(),
            'to_user_id' => $request->to_user_id,
            'status' => 'pending'
        ]);

        return response()->json(['message' => 'Intercambio enviado.', 'trade' => $trade]);
    }

    public function accept($id)
    {
        $trade = Trade::findOrFail($id);
        $trade->update(['status' => 'accepted']);
        return response()->json(['message' => 'Intercambio aceptado.']);
    }

    public function reject($id)
    {
        $trade = Trade::findOrFail($id);
        $trade->update(['status' => 'rejected']);
        return response()->json(['message' => 'Intercambio rechazado.']);
    }
}
