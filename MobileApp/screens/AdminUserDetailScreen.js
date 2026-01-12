import React, { useState, useEffect } from 'react';
import {
  View,
  Text,
  StyleSheet,
  ScrollView,
  TextInput,
  TouchableOpacity,
  Alert,
} from 'react-native';
import { getAdminUserDetail, adminSetUserRating } from '../services/api';

export default function AdminUserDetailScreen({ route }) {
  const { userId } = route.params;
  const [data, setData] = useState(null);
  const [newRating, setNewRating] = useState('');

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      const result = await getAdminUserDetail(userId);
      setData(result);
      setNewRating(result.user.averageRating.toString());
    } catch (error) {
      Alert.alert('Napaka', 'Ni mogoče naložiti podatkov');
    }
  };

  const handleSaveRating = async () => {
    const rating = parseFloat(newRating);
    if (isNaN(rating) || rating < 0 || rating > 5) {
      Alert.alert('Napaka', 'Ocena mora biti med 0 in 5');
      return;
    }

    try {
      await adminSetUserRating(userId, rating);
      Alert.alert('Uspeh', 'Ocena shranjena');
      loadData();
    } catch (error) {
      Alert.alert('Napaka', 'Ni mogoče shraniti ocene');
    }
  };

  if (!data) {
    return (
      <View style={styles.loading}>
        <Text>Nalaganje...</Text>
      </View>
    );
  }

  return (
    <ScrollView style={styles.container}>
      <View style={styles.card}>
        <Text style={styles.name}>
          {data.user.firstName} {data.user.lastName}
        </Text>
        <Text style={styles.email}>{data.user.email}</Text>

        <View style={styles.statsRow}>
          <View style={styles.stat}>
            <Text style={styles.statLabel}>Termini</Text>
            <Text style={styles.statValue}>{data.user.totalBookings}</Text>
          </View>
          <View style={styles.stat}>
            <Text style={styles.statLabel}>Prihodi</Text>
            <Text style={styles.statValue}>{data.user.completedBookings}</Text>
          </View>
          <View style={styles.stat}>
            <Text style={styles.statLabel}>Preklici</Text>
            <Text style={styles.statValue}>{data.user.cancelledBookings}</Text>
          </View>
        </View>

        <View style={styles.ratingSection}>
          <Text style={styles.label}>Povprečna ocena</Text>
          <View style={styles.ratingRow}>
            <TextInput
              style={styles.ratingInput}
              value={newRating}
              onChangeText={setNewRating}
              keyboardType="decimal-pad"
              placeholder="0.0 - 5.0"
            />
            <TouchableOpacity style={styles.saveBtn} onPress={handleSaveRating}>
              <Text style={styles.saveBtnText}>Shrani</Text>
            </TouchableOpacity>
          </View>
        </View>
      </View>

      <Text style={styles.sectionTitle}>Zgodovina terminov</Text>

      {data.prijave.length === 0 ? (
        <Text style={styles.noPrijave}>Ni terminov</Text>
      ) : (
        data.prijave.map((p) => (
          <View key={p.id} style={styles.prijavaCard}>
            <Text style={styles.terminDate}>
              {new Date(p.termin.zacetekCasa).toLocaleDateString('sl-SI')}
            </Text>
            <Text style={styles.terminLokacija}>{p.termin.lokacija}</Text>
            <View style={styles.terminRow}>
              <Text>{p.jePrispel ? '✅ Prispel' : '❌ Ni prispel'}</Text>
              {p.ocenaStranke && (
                <Text style={styles.ocena}>Ocena: {p.ocenaStranke}⭐</Text>
              )}
            </View>
          </View>
        ))
      )}
    </ScrollView>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#f5f5f5',
  },
  loading: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
  },
  card: {
    backgroundColor: '#fff',
    padding: 20,
    margin: 16,
    borderRadius: 12,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.1,
    shadowRadius: 4,
    elevation: 3,
  },
  name: {
    fontSize: 22,
    fontWeight: 'bold',
    color: '#333',
    marginBottom: 4,
  },
  email: {
    fontSize: 16,
    color: '#666',
    marginBottom: 20,
  },
  statsRow: {
    flexDirection: 'row',
    justifyContent: 'space-around',
    marginBottom: 20,
    paddingVertical: 15,
    borderTopWidth: 1,
    borderBottomWidth: 1,
    borderColor: '#f0f0f0',
  },
  stat: {
    alignItems: 'center',
  },
  statLabel: {
    fontSize: 12,
    color: '#666',
    marginBottom: 4,
  },
  statValue: {
    fontSize: 20,
    fontWeight: 'bold',
    color: '#007AFF',
  },
  ratingSection: {
    marginTop: 10,
  },
  label: {
    fontSize: 14,
    fontWeight: '600',
    color: '#333',
    marginBottom: 8,
  },
  ratingRow: {
    flexDirection: 'row',
    gap: 10,
  },
  ratingInput: {
    flex: 1,
    height: 45,
    borderWidth: 1,
    borderColor: '#ddd',
    borderRadius: 8,
    paddingHorizontal: 15,
    fontSize: 16,
    backgroundColor: '#f9f9f9',
  },
  saveBtn: {
    backgroundColor: '#007AFF',
    paddingHorizontal: 20,
    borderRadius: 8,
    justifyContent: 'center',
  },
  saveBtnText: {
    color: '#fff',
    fontWeight: 'bold',
  },
  sectionTitle: {
    fontSize: 18,
    fontWeight: 'bold',
    marginHorizontal: 16,
    marginTop: 10,
    marginBottom: 10,
  },
  noPrijave: {
    textAlign: 'center',
    color: '#666',
    marginTop: 20,
  },
  prijavaCard: {
    backgroundColor: '#fff',
    padding: 15,
    marginHorizontal: 16,
    marginBottom: 10,
    borderRadius: 8,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 1 },
    shadowOpacity: 0.05,
    shadowRadius: 2,
    elevation: 2,
  },
  terminDate: {
    fontSize: 16,
    fontWeight: 'bold',
    marginBottom: 4,
  },
  terminLokacija: {
    fontSize: 14,
    color: '#666',
    marginBottom: 8,
  },
  terminRow: {
    flexDirection: 'row',
    justifyContent: 'space-between',
  },
  ocena: {
    color: '#FF9500',
    fontWeight: 'bold',
  },
});