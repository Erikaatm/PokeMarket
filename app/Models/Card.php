<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;

/**
 * 
 *
 * @property int $id
 * @property int $user_id
 * @property string $namePokemon
 * @property string $pokemon_type
 * @property string $image
 * @property string $edition
 * @property int $graded
 * @property int|null $grade
 * @property float|null $price
 * @property int $is_tradeable
 * @property \Illuminate\Support\Carbon|null $created_at
 * @property \Illuminate\Support\Carbon|null $updated_at
 * @property-read \Illuminate\Database\Eloquent\Collection<int, \App\Models\User> $favoritedBy
 * @property-read int|null $favorited_by_count
 * @property-read \App\Models\Grading|null $grading
 * @property-read \App\Models\User $owner
 * @property-read \Illuminate\Database\Eloquent\Collection<int, \App\Models\Tag> $tags
 * @property-read int|null $tags_count
 * @property-read \Illuminate\Database\Eloquent\Collection<int, \App\Models\Trade> $trades
 * @property-read int|null $trades_count
 * @method static \Illuminate\Database\Eloquent\Builder<static>|Card newModelQuery()
 * @method static \Illuminate\Database\Eloquent\Builder<static>|Card newQuery()
 * @method static \Illuminate\Database\Eloquent\Builder<static>|Card query()
 * @method static \Illuminate\Database\Eloquent\Builder<static>|Card whereCreatedAt($value)
 * @method static \Illuminate\Database\Eloquent\Builder<static>|Card whereEdition($value)
 * @method static \Illuminate\Database\Eloquent\Builder<static>|Card whereGrade($value)
 * @method static \Illuminate\Database\Eloquent\Builder<static>|Card whereGraded($value)
 * @method static \Illuminate\Database\Eloquent\Builder<static>|Card whereId($value)
 * @method static \Illuminate\Database\Eloquent\Builder<static>|Card whereImage($value)
 * @method static \Illuminate\Database\Eloquent\Builder<static>|Card whereIsTradeable($value)
 * @method static \Illuminate\Database\Eloquent\Builder<static>|Card whereNamePokemon($value)
 * @method static \Illuminate\Database\Eloquent\Builder<static>|Card wherePokemonType($value)
 * @method static \Illuminate\Database\Eloquent\Builder<static>|Card wherePrice($value)
 * @method static \Illuminate\Database\Eloquent\Builder<static>|Card whereUpdatedAt($value)
 * @method static \Illuminate\Database\Eloquent\Builder<static>|Card whereUserId($value)
 * @mixin \Eloquent
 */
class Card extends Model
{
    use HasFactory;

    protected $fillable = ['user_id', 'namePokemon', 'pokemon_type', 'image', 'edition', 'graded', 'grade', 'price', 'is_tradeable'];

    public function owner()
    {
        return $this->belongsTo(User::class, 'user_id');
    }

    public function tags()
    {
        return $this->belongsToMany(Tag::class, 'card_tag');
    }

    public function favoritedBy()
    {
        return $this->belongsToMany(User::class, 'favorites');
    }

    public function trades()
    {
        return $this->belongsToMany(Trade::class, 'trade_cards');
    }

    public function grading()
    {
        return $this->hasOne(Grading::class);
    }
}