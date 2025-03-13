<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

return new class extends Migration
{
    /**
     * Run the migrations.
     */
    public function up()
    {
        Schema::create('favorites', function (Blueprint $table) {
            $table->id();
            $table->foreignId('user_id')->constrained()->onDelete('cascade'); // Usuario que marca como favorito
            $table->foreignId('card_id')->constrained()->onDelete('cascade'); // Carta marcada como favorita
            $table->timestamps();

            // Un usuario no puede marcar la misma carta como favorita más de una vez
            $table->unique(['user_id', 'card_id']);
        });
    }

    public function down()
    {
        Schema::dropIfExists('favorites');
    }
};
