import React, { useState, useEffect } from 'react';
import {
  View,
  Text,
  FlatList,
  StyleSheet,
  TouchableOpacity,
  Alert,
  RefreshControl,
} from 'react-native';
import { getAdminTermini, adminCheckIn, adminRateUser } from '../services/api';

export default function AdminTerminiScreen() {
  const [termini, setTermini] = useState([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    loadTermini();
  }, []);

  const loadTermini = async () => {
    try {
      setLoading(true);
      const data = await getAdminTermini();
      setTermini(data);
    } catch (error) {
      Alert.alert('Napaka', 'Ni mogoče naložiti terminov');
    } finally {
      setLoading(false);
    }
  };

  const handleCheckIn = async (prijavaId, userName) => {
    Alert.alert(
      'Check-in',
      `Označi ${userName} kot prisotnega?`,
      [
        { text: 'Prekliči', style: 'cancel' },
        {
          text: 'Check-in',
          onPress: async () => {
            try {
              await adminCheckIn(prijavaId);
              Alert.alert('Uspeh', 'Check-in uspešen');
              loadTermini();
            } catch (error) {
              Alert.alert('Napaka', 'Check-in ni uspel');
            }
          },
        },
      ]
    );
  };

  const handleRate = async (prijavaId, userName) => {
    Alert.prompt(
      'Oceni uporabnika',
      `Oceni ${userName} (1-5):`,
      [
        { text: 'Prekliči', style: 'cancel' },
        {
          text: 'Shrani',
          onPress: async (rating) => {
            const r = parseInt(rating);
            if (r < 1 || r > 5 || isNaN(r)) {
              Alert.alert('Napaka', 'Vnesi število med 1 in 5');
              return;
            }
            try {
              await adminRateUser(prijavaId, r);
              Alert.alert('Uspeh', 'Ocena shranjena');
              loadTermini();
            } catch (error) {
              Alert.alert('Napaka', 'Ocena ni shranjena');
            }
          },
        },
      ],
      'plain-text'
    );
  };

  const renderTermin = ({ item }) => (
    <View style={styles.card}>
      <View style={styles.header}>
        <Text style={styles.datum}>
          {new Date(item.zacetekCasa).toLocaleDateString('sl-SI', {
            weekday: 'short',
            day: 'numeric',
            month: 'short',
          })}
        </Text>
        <Text style={styles.cas}>
          {new Date(item.zacetekCasa).toLocaleTimeString('sl-SI', {
            hour: '2-digit',
            minute: '2-digit',
          })}
          -{' '}
          {new Date(item.konecCasa).toLocaleTimeString('sl-SI', {
            hour: '2-digit',
            minute: '2-digit',
          })}
        </Text>
      </View>
      <Text style={styles.lokacija}>📍 {item.lokacija.naslov}</Text>
      <Text style={styles.opis}>{item.opis}</Text>

      <Text style={styles.prijaveSectionTitle}>
        Prijave ({item.prijave.length}/{item.maksimalnoUporabnikov}):
      </Text>

      {item.prijave.length === 0 ? (
        <Text style={styles.noPrijave}>Ni prijav</Text>
      ) : (
        item.prijave.map((p) => (
          <View key={p.id} style={styles.prijavaRow}>
            <View style={styles.userInfo}>
              <Text style={styles.userName}>
                {p.uporabnik.firstName} {p.uporabnik.lastName}
              </Text>
              <Text style={styles.userEmail}>{p.uporabnik.email}</Text>
              {p.uporabnik.averageRating > 0 && (
                <Text style={styles.rating}>
                  ⭐ {p.uporabnik.averageRating.toFixed(1)}
                </Text>
              )}
            </View>

            <View style={styles.actions}>
              {!p.jePrispel ? (
                <TouchableOpacity
                  style={styles.checkInBtn}
                  onPress={() =>
                    handleCheckIn(
                      p.id,
                      `${p.uporabnik.firstName} ${p.uporabnik.lastName}`
                    )
                  }
                >
                  <Text style={styles.btnText}>✓</Text>
                </TouchableOpacity>
              ) : (
                <Text style={styles.checked}>✅</Text>
              )}

              {p.jePrispel && !p.ocenaStranke && (
                <TouchableOpacity
                  style={styles.rateBtn}
                  onPress={() =>
                    handleRate(
                      p.id,
                      `${p.uporabnik.firstName} ${p.uporabnik.lastName}`
                    )
                  }
                >
                  <Text style={styles.btnText}>⭐</Text>
                </TouchableOpacity>
              )}

              {p.ocenaStranke && (
                <Text style={styles.rated}>{p.ocenaStranke}⭐</Text>
              )}
            </View>
          </View>
        ))
      )}
    </View>
  );

  return (
    <View style={styles.container}>
      <FlatList
        data={termini}
        renderItem={renderTermin}
        keyExtractor={(item) => item.id.toString()}
        refreshControl={
          <RefreshControl refreshing={loading} onRefresh={loadTermini} />
        }
        contentContainerStyle={styles.listContent}
      />
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#f5f5f5',
  },
  listContent: {
    padding: 16,
  },
  card: {
    backgroundColor: '#fff',
    borderRadius: 12,
    padding: 16,
    marginBottom: 16,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.1,
    shadowRadius: 4,
    elevation: 3,
  },
  header: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    marginBottom: 8,
  },
  datum: {
    fontSize: 16,
    fontWeight: 'bold',
    color: '#333',
  },
  cas: {
    fontSize: 14,
    color: '#007AFF',
    fontWeight: '600',
  },
  lokacija: {
    fontSize: 14,
    color: '#666',
    marginBottom: 4,
  },
  opis: {
    fontSize: 13,
    color: '#999',
    marginBottom: 12,
    fontStyle: 'italic',
  },
  prijaveSectionTitle: {
    fontSize: 14,
    fontWeight: 'bold',
    color: '#333',
    marginTop: 8,
    marginBottom: 8,
  },
  noPrijave: {
    fontSize: 13,
    color: '#999',
    fontStyle: 'italic',
  },
  prijavaRow: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    paddingVertical: 8,
    borderBottomWidth: 1,
    borderBottomColor: '#f0f0f0',
  },
  userInfo: {
    flex: 1,
  },
  userName: {
    fontSize: 14,
    fontWeight: '600',
    color: '#333',
  },
  userEmail: {
    fontSize: 12,
    color: '#666',
  },
  rating: {
    fontSize: 12,
    color: '#FF9500',
  },
  actions: {
    flexDirection: 'row',
    gap: 8,
  },
  checkInBtn: {
    backgroundColor: '#34C759',
    width: 36,
    height: 36,
    borderRadius: 18,
    justifyContent: 'center',
    alignItems: 'center',
  },
  rateBtn: {
    backgroundColor: '#FF9500',
    width: 36,
    height: 36,
    borderRadius: 18,
    justifyContent: 'center',
    alignItems: 'center',
  },
  btnText: {
    color: '#fff',
    fontSize: 18,
    fontWeight: 'bold',
  },
  checked: {
    fontSize: 24,
  },
  rated: {
    fontSize: 14,
    color: '#FF9500',
    fontWeight: 'bold',
  },
});