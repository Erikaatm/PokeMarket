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
        Schema::table('users', function (Blueprint $table) {
            if (Schema::hasColumn('users', 'name')) {
                $table->dropColumn('name'); // Elimina 'name' si existe
            }

            if (!Schema::hasColumn('users', 'username')) {
                $table->string('username')->unique();
            }
        });
    }

    public function down()
    {
        Schema::table('users', function (Blueprint $table) {
            if (Schema::hasColumn('users', 'username')) {
                $table->dropColumn('username'); // Revertir cambios si se ejecuta 'rollback'
            }

            if (!Schema::hasColumn('users', 'name')) {
                $table->string('name'); // Volver a agregar 'name' en caso de rollback
            }
        });
    }
};
