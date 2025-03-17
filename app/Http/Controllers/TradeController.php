<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use App\Models\Trade;
use Illuminate\Support\Facades\Auth;

class TradeController extends Controller
{
    /**
     * Crear una solicitud de intercambio.
     */
    public function addTrade(Request $request)
    {
        $validated = $request->validate([
            'to_user_id' => 'required|exists:users,id|not_in:' . Auth::id(), // No puedes enviarte intercambios a ti mismo
        ]);

        $trade = Trade::create([
            'from_user_id' => Auth::id(),
            'to_user_id'   => $validated['to_user_id'],
            'status'       => 'pending',
        ]);

        return response()->json([
            'message' => 'Intercambio enviado con éxito.',
            'trade'   => $trade
        ], 201);
    }

    /**
     * Aceptar un intercambio.
     */
    public function acceptTrade($id)
    {
        $trade = Trade::findOrFail($id);

        if ($trade->to_user_id !== Auth::id()) {
            return response()->json(['message' => 'No autorizado para aceptar este intercambio.'], 403);
        }

        $trade->update(['status' => 'accepted']);

        return response()->json(['message' => 'Intercambio aceptado con éxito.']);
    }

    /**
     * Rechazar un intercambio.
     */
    public function rejectTrade($id)
    {
        $trade = Trade::findOrFail($id);

        if ($trade->to_user_id !== Auth::id()) {
            return response()->json(['message' => 'No autorizado para rechazar este intercambio.'], 403);
        }

        $trade->update(['status' => 'rejected']);

        return response()->json(['message' => 'Intercambio rechazado con éxito.']);
    }
}
