<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Foundation\Auth\User as Authenticatable;
use Illuminate\Notifications\Notifiable;
use Illuminate\Database\Eloquent\Relations\BelongsToMany;
use App\Models\Card;

/**
 * 
 *
 * @property \Illuminate\Database\Eloquent\Collection $favorites
 * @property int $id
 * @property string $email
 * @property string|null $email_verified_at
 * @property string $password
 * @property string|null $phone_num
 * @property string|null $address
 * @property string|null $remember_token
 * @property \Illuminate\Support\Carbon|null $created_at
 * @property \Illuminate\Support\Carbon|null $updated_at
 * @property string $username
 * @property string $role
 * @property-read \Illuminate\Database\Eloquent\Collection<int, Card> $cards
 * @property-read int|null $cards_count
 * @property-read int|null $favorites_count
 * @property-read \Illuminate\Database\Eloquent\Collection<int, \App\Models\Message> $messagesReceived
 * @property-read int|null $messages_received_count
 * @property-read \Illuminate\Database\Eloquent\Collection<int, \App\Models\Message> $messagesSent
 * @property-read int|null $messages_sent_count
 * @property-read \Illuminate\Notifications\DatabaseNotificationCollection<int, \Illuminate\Notifications\DatabaseNotification> $notifications
 * @property-read int|null $notifications_count
 * @property-read \Illuminate\Database\Eloquent\Collection<int, \App\Models\Trade> $tradesReceived
 * @property-read int|null $trades_received_count
 * @property-read \Illuminate\Database\Eloquent\Collection<int, \App\Models\Trade> $tradesSent
 * @property-read int|null $trades_sent_count
 * @method static \Database\Factories\UserFactory factory($count = null, $state = [])
 * @method static \Illuminate\Database\Eloquent\Builder<static>|User newModelQuery()
 * @method static \Illuminate\Database\Eloquent\Builder<static>|User newQuery()
 * @method static \Illuminate\Database\Eloquent\Builder<static>|User query()
 * @method static \Illuminate\Database\Eloquent\Builder<static>|User whereAddress($value)
 * @method static \Illuminate\Database\Eloquent\Builder<static>|User whereCreatedAt($value)
 * @method static \Illuminate\Database\Eloquent\Builder<static>|User whereEmail($value)
 * @method static \Illuminate\Database\Eloquent\Builder<static>|User whereEmailVerifiedAt($value)
 * @method static \Illuminate\Database\Eloquent\Builder<static>|User whereId($value)
 * @method static \Illuminate\Database\Eloquent\Builder<static>|User wherePassword($value)
 * @method static \Illuminate\Database\Eloquent\Builder<static>|User wherePhoneNum($value)
 * @method static \Illuminate\Database\Eloquent\Builder<static>|User whereRememberToken($value)
 * @method static \Illuminate\Database\Eloquent\Builder<static>|User whereRole($value)
 * @method static \Illuminate\Database\Eloquent\Builder<static>|User whereUpdatedAt($value)
 * @method static \Illuminate\Database\Eloquent\Builder<static>|User whereUsername($value)
 * @mixin \Eloquent
 */
class User extends Authenticatable
{
    use HasFactory, Notifiable;

    protected $fillable = ['username', 'email', 'password', 'phone_num', 'address', 'role'];

    public function cards()
    {
        return $this->hasMany(Card::class);
    }

    /**
     * @return BelongsToMany
     */
    public function favorites()
    {
        return $this->belongsToMany(Card::class, 'favorites', 'user_id', 'card_id')->withTimestamps();
    }

    public function tradesSent()
    {
        return $this->hasMany(Trade::class, 'from_user_id');
    }

    public function tradesReceived()
    {
        return $this->hasMany(Trade::class, 'to_user_id');
    }

    public function messagesSent()
    {
        return $this->hasMany(Message::class, 'from_user_id');
    }

    public function messagesReceived()
    {
        return $this->hasMany(Message::class, 'to_user_id');
    }
}
