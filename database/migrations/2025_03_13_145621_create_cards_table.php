<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

return new class extends Migration
{
    /**
     * Run the migrations.
     */
    public function up(): void
    {
        Schema::create('cards', function (Blueprint $table) {
            $table->id();
            $table->foreignId('user_id')->constrained()->onDelete('cascade'); // Este es el dueño de la carta
            $table->string('namePokemon');
            $table->string('pokemon_type'); // El tipo que es el pokemon de la carta
            $table->string('image'); // Imagen de la carta
            $table->string('edition'); // Esta es la edición de la carta
            $table->boolean('graded')->default(false); // Si la carta está gradeada o no
            $table->integer('grade')->nullable(); // Si la carta está gradeada o no
            $table->float('price')->nullable(); // Precio de venta de la cara
            $table->boolean('is_tradeable')->default(false); // Si la carta se puede tradear por otra
            $table->timestamps();
        });
    }

    /**
     * Reverse the migrations.
     */
    public function down(): void
    {
        Schema::dropIfExists('cards');
    }
};
