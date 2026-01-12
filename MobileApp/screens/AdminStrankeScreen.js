import React, { useState, useEffect } from 'react';
import {
  View,
  Text,
  FlatList,
  StyleSheet,
  TouchableOpacity,
  RefreshControl,
  Alert,
} from 'react-native';
import { getAdminUsers } from '../services/api';

export default function AdminStrankeScreen({ navigation }) {
  const [stranke, setStranke] = useState([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    loadStranke();
  }, []);

  const loadStranke = async () => {
    try {
      setLoading(true);
      const data = await getAdminUsers();
      setStranke(data);
    } catch (error) {
      Alert.alert('Napaka', 'Ni mogoče naložiti strank');
    } finally {
      setLoading(false);
    }
  };

  const renderStranka = ({ item }) => (
    <TouchableOpacity
      style={styles.card}
      onPress={() =>
        navigation.navigate('AdminUserDetail', { userId: item.id })
      }
    >
      <View style={styles.row}>
        <View style={styles.userInfo}>
          <Text style={styles.userName}>
            {item.firstName} {item.lastName}
          </Text>
          <Text style={styles.userEmail}>{item.email}</Text>
        </View>
        <View style={styles.stats}>
          <Text style={styles.rating}>
            {item.averageRating > 0
              ? `⭐ ${item.averageRating.toFixed(1)}`
              : '⭐ -'}
          </Text>
          <Text style={styles.reliability}>
            {item.totalBookings > 0
              ? `${((item.completedBookings / item.totalBookings) * 100).toFixed(
                  0
                )}% zanesljiv`
              : 'Ni podatkov'}
          </Text>
        </View>
      </View>
    </TouchableOpacity>
  );

  return (
    <View style={styles.container}>
      <FlatList
        data={stranke}
        renderItem={renderStranka}
        keyExtractor={(item) => item.id}
        refreshControl={
          <RefreshControl refreshing={loading} onRefresh={loadStranke} />
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
    marginBottom: 12,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.1,
    shadowRadius: 4,
    elevation: 3,
  },
  row: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
  },
  userInfo: {
    flex: 1,
  },
  userName: {
    fontSize: 16,
    fontWeight: 'bold',
    color: '#333',
    marginBottom: 4,
  },
  userEmail: {
    fontSize: 14,
    color: '#666',
  },
  stats: {
    alignItems: 'flex-end',
  },
  rating: {
    fontSize: 16,
    fontWeight: 'bold',
    color: '#FF9500',
    marginBottom: 4,
  },
  reliability: {
    fontSize: 12,
    color: '#666',
  },
});